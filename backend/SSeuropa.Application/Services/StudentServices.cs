using System.Security.Cryptography;
using SSeuropa.Application.DTOs;
using SSeuropa.Application.Interfaces;
using SSeuropa.Domain.Entities;
using SSeuropa.Domain.Enums;
using SSeuropa.Domain.Exceptions;

namespace SSeuropa.Application.Services;

/// <summary>
/// Which courses a student may open.
///
/// A student may join the courses assigned to any of their batches plus any course marked "open to all"
/// (e.g. a free demo course). Courses a student was already enrolled in before batches existed
/// stay visible until the team places the student in a batch. From then on the batch decides.
/// Team members (Teacher, Staff, Admin, Super Admin) are never limited.
/// </summary>
public class CourseAccessService(
    IUserRepository users,
    IBatchRepository batches,
    ICourseRepository courses,
    IEnrollmentRepository enrollments)
{
    /// <summary>Courses the user may join, or null when the user is a team member (no limit).</summary>
    public async Task<HashSet<Guid>?> GetAllowedCourseIdsAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await users.GetByIdAsync(userId, ct);
        if (user is null) return [];
        if (user.Role != UserRole.User) return null;
        return await AllowedForAsync(user, ct);
    }

    /// <summary>Courses the student sees: the ones they may join plus the ones they are actively enrolled in.</summary>
    public async Task<HashSet<Guid>?> GetVisibleCourseIdsAsync(Guid userId, CancellationToken ct = default)
    {
        var allowed = await GetAllowedCourseIdsAsync(userId, ct);
        if (allowed is null) return null;
        foreach (var e in await enrollments.GetByUserAsync(userId, ct))
            if (e.Status == EnrollmentStatus.Active) allowed.Add(e.CourseId);
        return allowed;
    }

    public async Task<bool> CanEnrollAsync(Guid userId, Guid courseId, CancellationToken ct = default)
    {
        var allowed = await GetAllowedCourseIdsAsync(userId, ct);
        return allowed is null || allowed.Contains(courseId);
    }

    public async Task<bool> CanSeeAsync(Guid userId, Guid courseId, CancellationToken ct = default)
    {
        var visible = await GetVisibleCourseIdsAsync(userId, ct);
        return visible is null || visible.Contains(courseId);
    }

    private async Task<HashSet<Guid>> AllowedForAsync(User user, CancellationToken ct)
    {
        var allowed = (await courses.GetAllAsync(ct)).Where(c => c.OpenToAll && !c.IsDeleted).Select(c => c.Id).ToHashSet();
        allowed.UnionWith(await BatchCoursesAsync(user, ct));
        return allowed;
    }

    /// <summary>The courses of every batch the student is in (a student may be in several batches).</summary>
    private async Task<HashSet<Guid>> BatchCoursesAsync(User user, CancellationToken ct)
    {
        var result = new HashSet<Guid>();
        foreach (var batchId in user.AllBatchIds())
            if (await batches.GetByIdAsync(batchId, ct) is { } batch)
                result.UnionWith(batch.CourseIds);
        return result;
    }

    /// <summary>
    /// Makes the student's enrollments match their batch: every batch course is opened (no need to
    /// press "Enroll"), and courses that are neither in the batch nor open to all are closed.
    /// Progress is never deleted, so moving a student back restores where they were.
    /// </summary>
    public async Task SyncEnrollmentsAsync(User user, CancellationToken ct = default)
    {
        if (user.Role != UserRole.User) return;

        var allowed = await AllowedForAsync(user, ct);
        var batchCourses = (await BatchCoursesAsync(user, ct)).ToList();
        var mine = (await enrollments.GetByUserAsync(user.Id, ct))
            .GroupBy(e => e.CourseId)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.Status == EnrollmentStatus.Active).First());

        foreach (var courseId in batchCourses)
        {
            var course = await courses.GetByIdAsync(courseId, ct);
            if (course is null || course.IsDeleted) continue;

            if (mine.TryGetValue(courseId, out var existing))
            {
                if (existing.Status == EnrollmentStatus.Active) continue;
                existing.Status = EnrollmentStatus.Active;
                existing.ActivatedAtUtc = DateTime.UtcNow;
                await enrollments.UpdateAsync(existing, ct);
            }
            else
            {
                await enrollments.AddAsync(new Enrollment
                {
                    UserId = user.Id,
                    CourseId = courseId,
                    Status = EnrollmentStatus.Active,
                    ActivatedAtUtc = DateTime.UtcNow
                }, ct);
            }
            course.EnrolledCount += 1;
            await courses.UpdateAsync(course, ct);
        }

        foreach (var e in mine.Values.Where(e => e.Status == EnrollmentStatus.Active && !allowed.Contains(e.CourseId)))
        {
            e.Status = EnrollmentStatus.Cancelled;
            await enrollments.UpdateAsync(e, ct);
            if (await courses.GetByIdAsync(e.CourseId, ct) is { } course)
            {
                course.EnrolledCount = Math.Max(0, course.EnrolledCount - 1);
                await courses.UpdateAsync(course, ct);
            }
        }
    }
}

/// <summary>
/// Students and batches in the admin panel.
///
/// Who can do what:
///   • Admin and Super Admin: everything (students and batches).
///   • Staff (admissions): add students, edit them, place them in a batch, reset passwords.
///   • Teacher: read-only view of the batches they lead and the students in them.
/// </summary>
public class StudentService(
    IUserRepository users,
    IBatchRepository batches,
    ICourseRepository courses,
    IEnrollmentRepository enrollments,
    IProgressRepository progress,
    IPasswordHasher hasher,
    IEmailService email,
    IAuditLogRepository audit,
    ICurrentUserService current,
    CourseAccessService access)
{
    // The endpoints already check each person's Students / Batches permission; here any team member may act.
    private bool IsAdmin => current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Staff, UserRole.Mentor);
    private bool CanManageStudents => current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Staff, UserRole.Mentor);
    // A Teacher sees only the batches they teach, unless the Super Admin gave them full access.
    private bool TeacherSeesOwnStudentsOnly => current.Role == UserRole.Mentor && current.ModuleAccess(Common.TeamPermissions.Students) != Common.AccessLevel.Full;
    private bool TeacherSeesOwnBatchesOnly => current.Role == UserRole.Mentor && current.ModuleAccess(Common.TeamPermissions.Batches) != Common.AccessLevel.Full;

    // ══════════════════════════════════════════════════════════════════════
    //  Students
    // ══════════════════════════════════════════════════════════════════════

    public async Task<IReadOnlyList<StudentDto>> GetStudentsAsync(CancellationToken ct = default)
    {
        EnsureCanView();
        var allBatches = (await batches.GetAllAsync(ct)).ToDictionary(b => b.Id);
        var students = (await users.GetAllAsync(ct)).Where(u => u.Role == UserRole.User);

        if (TeacherSeesOwnStudentsOnly)
        {
            var mine = allBatches.Values.Where(b => b.MentorId == current.UserId).Select(b => b.Id).ToHashSet();
            // Plus paid students still waiting for their login, so a Teacher can send it.
            students = students.Where(u => !u.IsDeleted && (u.AwaitingLogin || u.AllBatchIds().Any(mine.Contains)));
        }

        var allEnrollments = await enrollments.GetAllAsync(ct);
        var activeByUser = allEnrollments.Where(e => e.Status == EnrollmentStatus.Active)
            .GroupBy(e => e.UserId).ToDictionary(g => g.Key, g => g.Select(e => e.CourseId).ToHashSet());
        var progressByUser = (await progress.GetAllAsync(ct))
            .GroupBy(p => p.UserId).ToDictionary(g => g.Key, g => g.ToList());

        return students
            .OrderByDescending(u => u.CreatedAtUtc)
            .Select(u =>
            {
                var active = activeByUser.GetValueOrDefault(u.Id) ?? [];
                var records = (progressByUser.GetValueOrDefault(u.Id) ?? []).Where(p => active.Contains(p.CourseId)).ToList();
                var percent = records.Count == 0 ? 0 : (int)Math.Round(records.Average(p => (double)p.CompletionPercent));
                return Map(u, BatchesOf(u, allBatches), active.Count, percent);
            })
            .ToList();
    }

    public async Task<StudentPasswordResultDto> CreateStudentAsync(CreateStudentRequest request, CancellationToken ct = default)
    {
        EnsureCanManageStudents();
        var emailAddress = (request.Email ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(emailAddress))
            throw new ValidationDomainException("First name and email are required.");
        if (!emailAddress.Contains('@') || emailAddress.StartsWith('@') || emailAddress.EndsWith('@'))
            throw new ValidationDomainException("Enter a valid email address.");
        if (await users.GetByEmailAsync(emailAddress, ct) is not null)
            throw new ConflictException("An account with this email already exists.");

        var chosen = await ResolveBatchesAsync(request.BatchIds, request.BatchId, ct);
        var password = GeneratePassword();
        var user = new User
        {
            Email = emailAddress,
            PasswordHash = hasher.Hash(password),
            FirstName = request.FirstName.Trim(),
            LastName = (request.LastName ?? "").Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
            PreferredLanguage = request.PreferredLanguage,
            Role = UserRole.User,
            Status = AccountStatus.Active,
            EmailVerified = true,
            AddedByStaff = true,
            MustChangePassword = true
        };
        user.SetBatches(chosen.Select(b => b.Id));
        await users.AddAsync(user, ct);
        await access.SyncEnrollmentsAsync(user, ct);

        var names = NamesOf(chosen);
        await email.SendStudentWelcomeAsync(user.Email, user.FirstName, password, names, isReset: false, ct);
        await LogAsync("student_created", $"Created student {user.Email}{(names is null ? "" : $" in {names}")}", ct);

        return new StudentPasswordResultDto(await GetDtoAsync(user, ct), password);
    }

    public async Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentRequest request, CancellationToken ct = default)
    {
        EnsureCanManageStudents();
        var user = await GetStudentAsync(id, ct);
        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ValidationDomainException("First name is required.");

        var chosen = await ResolveBatchesAsync(request.BatchIds, request.BatchId, ct);
        var before = user.AllBatchIds().ToHashSet();
        var batchChanged = !before.SetEquals(chosen.Select(b => b.Id));
        if (batchChanged && chosen.Count > 0 && WaitingForPayment(user))
            throw new ValidationDomainException(PaymentFirstMessage(user));

        user.FirstName = request.FirstName.Trim();
        user.LastName = (request.LastName ?? "").Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        if (request.PreferredLanguage.HasValue) user.PreferredLanguage = request.PreferredLanguage.Value;
        user.SetBatches(chosen.Select(b => b.Id));
        await users.UpdateAsync(user, ct);
        if (batchChanged) await access.SyncEnrollmentsAsync(user, ct);

        await LogAsync("student_updated", $"Updated student {user.Email}{(batchChanged ? $" → {NamesOf(chosen) ?? "no batch"}" : "")}", ct);
        return await GetDtoAsync(user, ct);
    }

    /// <summary>
    /// Bulk batch change for several students:
    ///   "replace" (default) — the batch becomes their only batch (no batch when BatchId is null);
    ///   "add" — they also join the batch and keep their other batches;
    ///   "remove" — they leave the batch and keep their other batches.
    /// </summary>
    public async Task<MoveStudentsResultDto> MoveStudentsAsync(MoveStudentsRequest request, CancellationToken ct = default)
    {
        EnsureCanManageStudents();
        if (request.StudentIds is null || request.StudentIds.Count == 0)
            throw new ValidationDomainException("Choose at least one student.");

        var mode = (request.Mode ?? "replace").Trim().ToLowerInvariant();
        if (mode is not ("replace" or "add" or "remove"))
            throw new ValidationDomainException("Choose whether to move, add or remove.");
        var batch = await GetBatchOrNullAsync(request.BatchId, ct);
        if (batch is null && mode != "replace")
            throw new ValidationDomainException("Choose a batch.");

        // A batch opens lessons, so students waiting for payment / their login only get one through "Mark paid" / "Send login".
        if (batch is not null && mode != "remove")
        {
            var waiting = new List<string>();
            foreach (var id in request.StudentIds.Distinct())
                if (await users.GetByIdAsync(id, ct) is { } u && WaitingForPayment(u)) waiting.Add(u.FullName);
            if (waiting.Count > 0)
                throw new ValidationDomainException($"Mark these students as paid first (or send their login): {string.Join(", ", waiting)}. Then they can be placed in a batch.");
        }

        var changed = 0;
        foreach (var id in request.StudentIds.Distinct())
        {
            var user = await GetStudentAsync(id, ct);
            var mine = user.AllBatchIds().ToList();
            var next = mode switch
            {
                "add" => mine.Contains(batch!.Id) ? mine : [.. mine, batch.Id],
                "remove" => mine.Where(b => b != batch!.Id).ToList(),
                _ => batch is null ? [] : new List<Guid> { batch.Id },
            };
            if (mine.ToHashSet().SetEquals(next)) continue;
            user.SetBatches(next);
            await users.UpdateAsync(user, ct);
            await access.SyncEnrollmentsAsync(user, ct);
            changed++;
        }

        var verb = mode switch { "add" => "Added", "remove" => "Removed", _ => "Moved" };
        await LogAsync("students_moved", $"{verb} {changed} student(s) {(mode == "remove" ? "from" : "to")} batch '{batch?.Name ?? "none"}'", ct);
        return new MoveStudentsResultDto(changed, batch?.Name);
    }

    public async Task<StudentDto> SetStudentStatusAsync(Guid id, bool isActive, CancellationToken ct = default)
    {
        EnsureCanManageStudents();
        var user = await GetStudentAsync(id, ct);
        user.Status = isActive ? AccountStatus.Active : AccountStatus.Suspended;
        await users.UpdateAsync(user, ct);
        await LogAsync(isActive ? "student_activated" : "student_suspended", $"{(isActive ? "Activated" : "Suspended")} student {user.Email}", ct);
        return await GetDtoAsync(user, ct);
    }

    /// <summary>Issues a new temporary password, emails it, and asks the student to choose a new one at next sign-in.</summary>
    public async Task<StudentPasswordResultDto> ResetStudentPasswordAsync(Guid id, CancellationToken ct = default)
    {
        EnsureCanManageStudents();
        var user = await GetStudentAsync(id, ct);
        if (user.AwaitingLogin)
            throw new ValidationDomainException("This student hasn't received a login yet. Use “Send login” to choose their batch and email it.");
        var password = GeneratePassword();
        user.PasswordHash = hasher.Hash(password);
        user.MustChangePassword = true;
        await users.UpdateAsync(user, ct);

        var batchNames = NamesOf(await BatchesOfAsync(user, ct));
        await email.SendStudentWelcomeAsync(user.Email, user.FirstName, password, batchNames, isReset: true, ct);
        await LogAsync("student_password_reset", $"Reset password for student {user.Email}", ct);
        return new StudentPasswordResultDto(await GetDtoAsync(user, ct), password);
    }

    // ══════════════════════════════════════════════════════════════════════
    //  Paid students: enquiry → account → access
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Called when an enquiry is marked Paid. Creates the student account (no email yet) and returns its id.
    /// If a student already uses this email (e.g. they signed up themselves), that account is linked instead.
    /// The student shows under Students → "New (paid)" until someone gives access.
    /// </summary>
    public async Task<Guid> CreateFromEnquiryAsync(Enquiry enquiry, CancellationToken ct = default)
    {
        var emailAddress = (enquiry.Email ?? "").Trim().ToLowerInvariant();
        if (!emailAddress.Contains('@'))
            throw new ValidationDomainException("This enquiry has no valid email, so no student account can be made. Fix the email first.");

        var existing = await users.GetByEmailAsync(emailAddress, ct);
        if (existing is not null)
        {
            if (existing.Role != UserRole.User)
                throw new ValidationDomainException("This email belongs to a team member, so it can't become a student account.");
            if (existing.IsDeleted)
                throw new ConflictException("A removed student account uses this email. Restore it or use another email.");

            if (existing.AllBatchIds().Count == 0) existing.AwaitingLogin = true;
            if (string.IsNullOrWhiteSpace(existing.PhoneNumber) && !string.IsNullOrWhiteSpace(enquiry.Phone))
                existing.PhoneNumber = enquiry.Phone.Trim();
            await users.UpdateAsync(existing, ct);
            await LogAsync("student_linked_to_enquiry", $"Paid enquiry linked to existing student {existing.Email}", ct);
            return existing.Id;
        }

        var name = (enquiry.FullName ?? "").Trim();
        var space = name.IndexOf(' ');
        var user = new User
        {
            Email = emailAddress,
            // Nobody knows this password; the real one is emailed when access is given.
            PasswordHash = hasher.Hash(GeneratePassword()),
            FirstName = space > 0 ? name[..space] : (name.Length > 0 ? name : emailAddress.Split('@')[0]),
            LastName = space > 0 ? name[(space + 1)..].Trim() : "",
            PhoneNumber = string.IsNullOrWhiteSpace(enquiry.Phone) ? null : enquiry.Phone.Trim(),
            PreferredLanguage = Enum.TryParse<PreferredLanguage>(enquiry.PreferredLanguage, true, out var lang) ? lang : PreferredLanguage.English,
            Role = UserRole.User,
            Status = AccountStatus.Active,
            EmailVerified = true,
            AddedByStaff = true,
            MustChangePassword = true,
            AwaitingLogin = true
        };
        await users.AddAsync(user, ct);
        await LogAsync("student_created_from_enquiry", $"Created student {user.Email} from a paid enquiry", ct);
        return user.Id;
    }

    /// <summary>
    /// Gives a paid student access: places them in the chosen batches (their courses open) and, for accounts
    /// the team made, emails a one-time password. Students who signed up themselves keep their own password.
    ///
    /// Anyone with Students "Full" access may do this for any student (e.g. a self sign-up who paid by phone).
    /// With "View" access (a Teacher by default) it is only allowed for students waiting for their login,
    /// and a Teacher can only use the batches they teach.
    /// </summary>
    public async Task<StudentAccessResultDto> GiveAccessAsync(Guid id, GiveAccessRequest request, CancellationToken ct = default)
    {
        EnsureCanView();
        var user = await GetStudentAsync(id, ct);
        if (user.IsDeleted) throw new ValidationDomainException("This student was removed.");

        var fullAccess = current.Role == UserRole.SuperAdmin || current.ModuleAccess(Common.TeamPermissions.Students) == Common.AccessLevel.Full;
        if (!fullAccess && !user.AwaitingLogin)
            throw new UnauthorizedAccessDomainException("You have view-only access to Students. Ask the Super Admin if you need to give access to this student.");

        var chosen = await ResolveBatchesAsync(request.BatchIds, null, ct);
        if (chosen.Count == 0)
            throw new ValidationDomainException("Choose at least one batch. The batch decides which courses the student can open.");
        if (TeacherSeesOwnBatchesOnly && chosen.Any(b => b.MentorId != current.UserId))
            throw new ValidationDomainException("You can only place students in the batches you teach.");

        user.SetBatches(user.AllBatchIds().Concat(chosen.Select(b => b.Id)));
        user.AwaitingLogin = false;
        if (user.Status is AccountStatus.Suspended or AccountStatus.Disabled) user.Status = AccountStatus.Active;

        string? password = null;
        if (user.AddedByStaff)
        {
            password = GeneratePassword();
            user.PasswordHash = hasher.Hash(password);
            user.MustChangePassword = true;
        }
        await users.UpdateAsync(user, ct);
        await access.SyncEnrollmentsAsync(user, ct);

        var names = NamesOf(await BatchesOfAsync(user, ct));
        if (password is not null)
            await email.SendStudentWelcomeAsync(user.Email, user.FirstName, password, names, isReset: false, ct);
        await LogAsync("student_access_given", $"Gave access to {user.Email} in {names}{(password is null ? "" : " and emailed login details")}", ct);

        return new StudentAccessResultDto(await GetDtoAsync(user, ct), password);
    }

    // ══════════════════════════════════════════════════════════════════════
    //  Batches
    // ══════════════════════════════════════════════════════════════════════

    public async Task<IReadOnlyList<BatchDto>> GetBatchesAsync(CancellationToken ct = default)
    {
        EnsureCanView();
        var list = await batches.GetAllAsync(ct);
        if (TeacherSeesOwnBatchesOnly) list = list.Where(b => b.MentorId == current.UserId).ToList();

        var allUsers = await users.GetAllAsync(ct);
        var allCourses = (await courses.GetAllAsync(ct)).ToDictionary(c => c.Id);
        return list.Select(b => MapBatch(b, allUsers, allCourses)).ToList();
    }

    /// <summary>Team members who can lead a batch (Teachers first), for the batch form.</summary>
    public async Task<IReadOnlyList<TeacherOptionDto>> GetTeacherOptionsAsync(CancellationToken ct = default)
    {
        EnsureCanView();
        return (await users.GetAllAsync(ct))
            .Where(u => !u.IsDeleted && u.Role is UserRole.Mentor or UserRole.Admin or UserRole.SuperAdmin)
            .OrderBy(u => u.Role == UserRole.Mentor ? 0 : 1).ThenBy(u => u.FullName)
            .Select(u => new TeacherOptionDto(u.Id, u.FullName, u.Role))
            .ToList();
    }

    public async Task<BatchDto> CreateBatchAsync(SaveBatchRequest request, CancellationToken ct = default)
    {
        EnsureAdmin();
        var batch = new Batch();
        await ApplyAsync(batch, request, ct);
        await batches.AddAsync(batch, ct);
        await LogAsync("batch_created", $"Created batch '{batch.Name}' with {batch.CourseIds.Count} course(s)", ct);
        return await GetBatchDtoAsync(batch, ct);
    }

    public async Task<BatchDto> UpdateBatchAsync(Guid id, SaveBatchRequest request, CancellationToken ct = default)
    {
        EnsureAdmin();
        var batch = await batches.GetByIdAsync(id, ct) ?? throw new NotFoundException("Batch", id);
        var before = batch.CourseIds.ToHashSet();
        await ApplyAsync(batch, request, ct);
        await batches.UpdateAsync(batch, ct);

        // New or removed courses: open/close them for every student in the batch.
        if (!before.SetEquals(batch.CourseIds))
            foreach (var student in await StudentsInAsync(batch.Id, ct))
                await access.SyncEnrollmentsAsync(student, ct);

        await LogAsync("batch_updated", $"Updated batch '{batch.Name}'", ct);
        return await GetBatchDtoAsync(batch, ct);
    }

    public async Task DeleteBatchAsync(Guid id, CancellationToken ct = default)
    {
        EnsureAdmin();
        var batch = await batches.GetByIdAsync(id, ct) ?? throw new NotFoundException("Batch", id);
        var students = await StudentsInAsync(batch.Id, ct);
        if (students.Count > 0)
            throw new ValidationDomainException($"This batch still has {students.Count} student(s). Move them to another batch or remove them from this batch first.");
        await batches.DeleteAsync(batch.Id, ct);
        await LogAsync("batch_deleted", $"Deleted batch '{batch.Name}'", ct);
    }

    // ══════════════════════════════════════════════════════════════════════
    //  Helpers
    // ══════════════════════════════════════════════════════════════════════

    private async Task ApplyAsync(Batch batch, SaveBatchRequest request, CancellationToken ct)
    {
        var name = (request.Name ?? "").Trim();
        if (name.Length == 0) throw new ValidationDomainException("Give the batch a name.");
        if (name.Length > 80) throw new ValidationDomainException("The batch name can be at most 80 characters.");
        var duplicate = (await batches.GetAllAsync(ct)).Any(b => b.Id != batch.Id && string.Equals(b.Name.Trim(), name, StringComparison.OrdinalIgnoreCase));
        if (duplicate) throw new ConflictException($"A batch called '{name}' already exists.");

        if (request.MentorId is { } mentorId)
        {
            var mentor = await users.GetByIdAsync(mentorId, ct);
            if (mentor is null || mentor.IsDeleted || mentor.Role is not (UserRole.Mentor or UserRole.Admin or UserRole.SuperAdmin))
                throw new ValidationDomainException("Choose a teacher from your team.");
        }

        var courseIds = (request.CourseIds ?? []).Distinct().ToList();
        foreach (var courseId in courseIds)
        {
            var course = await courses.GetByIdAsync(courseId, ct);
            if (course is null || course.IsDeleted) throw new ValidationDomainException("One of the chosen courses no longer exists.");
        }

        batch.Name = name;
        // Level is no longer asked for: keep what the batch had (new batches: "A0").
        if (!string.IsNullOrWhiteSpace(request.Level)) batch.Level = request.Level.Trim().ToUpperInvariant();
        batch.Description = (request.Description ?? "").Trim();
        batch.MentorId = request.MentorId;
        batch.CourseIds = courseIds;
        batch.StartDateUtc = request.StartDateUtc;
        (batch.StartTime, batch.EndTime) = ParseTiming(request.StartTime, request.EndTime);
    }

    /// <summary>Batch class time as "HH:mm" (24-hour). Both or neither; the end must be after the start.</summary>
    private static (string? Start, string? End) ParseTiming(string? start, string? end)
    {
        static TimeOnly? Parse(string? value, string label)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (!TimeOnly.TryParseExact(value.Trim(), ["HH:mm", "H:mm"], out var t))
                throw new ValidationDomainException($"{label} must be a time like 11:00 or 17:30.");
            return t;
        }

        var s = Parse(start, "Start time");
        var e = Parse(end, "End time");
        if (s is null && e is null) return (null, null);
        if (s is null || e is null) throw new ValidationDomainException("Give both a start and an end time, or leave both empty.");
        if (e <= s) throw new ValidationDomainException("The end time must be after the start time.");
        return (s.Value.ToString("HH:mm"), e.Value.ToString("HH:mm"));
    }

    /// <summary>
    /// Paid enquiry waiting for "Send login", or a self sign-up with no batch (payment not taken yet).
    /// These students get a batch only through Give access, never through Edit or bulk move.
    /// </summary>
    private static bool WaitingForPayment(User u) => u.AwaitingLogin || (!u.AddedByStaff && u.AllBatchIds().Count == 0);

    private static string PaymentFirstMessage(User u) => u.AwaitingLogin
        ? "Use “Send login” to place this student in a batch: it also emails their one-time password."
        : "Mark this student as paid first. “Mark paid” places them in a batch and opens their lessons.";

    private async Task<List<User>> StudentsInAsync(Guid batchId, CancellationToken ct) =>
        (await users.GetAllAsync(ct)).Where(u => u.Role == UserRole.User && u.IsInBatch(batchId) && !u.IsDeleted).ToList();

    private async Task<Batch?> GetBatchOrNullAsync(Guid? batchId, CancellationToken ct, bool throwIfMissing = true)
    {
        if (batchId is null) return null;
        var batch = await batches.GetByIdAsync(batchId.Value, ct);
        if (batch is null && throwIfMissing) throw new ValidationDomainException("That batch no longer exists.");
        return batch;
    }

    /// <summary>The batches chosen in a form: the list when given, otherwise the single (older) BatchId field.</summary>
    private async Task<List<Batch>> ResolveBatchesAsync(List<Guid>? batchIds, Guid? batchId, CancellationToken ct)
    {
        var ids = batchIds ?? (batchId is { } single ? [single] : []);
        var result = new List<Batch>();
        foreach (var id in ids.Where(id => id != Guid.Empty).Distinct())
            result.Add(await GetBatchOrNullAsync(id, ct) ?? throw new ValidationDomainException("That batch no longer exists."));
        return result;
    }

    private async Task<List<Batch>> BatchesOfAsync(User user, CancellationToken ct)
    {
        var result = new List<Batch>();
        foreach (var id in user.AllBatchIds())
            if (await batches.GetByIdAsync(id, ct) is { } b) result.Add(b);
        return result;
    }

    private static List<Batch> BatchesOf(User user, Dictionary<Guid, Batch> allBatches) =>
        user.AllBatchIds().Select(allBatches.GetValueOrDefault).OfType<Batch>().ToList();

    /// <summary>"Morning batch" or "Morning batch, Evening batch" (null = no batch), used in emails and the log.</summary>
    private static string? NamesOf(IReadOnlyCollection<Batch> list) =>
        list.Count == 0 ? null : string.Join(", ", list.Select(b => b.Name));

    private async Task<User> GetStudentAsync(Guid id, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(id, ct) ?? throw new NotFoundException("Student", id);
        if (user.Role != UserRole.User) throw new ValidationDomainException("This account is a team member, not a student.");
        return user;
    }

    private async Task<StudentDto> GetDtoAsync(User user, CancellationToken ct)
    {
        var mine = await BatchesOfAsync(user, ct);
        var active = (await enrollments.GetByUserAsync(user.Id, ct)).Where(e => e.Status == EnrollmentStatus.Active).Select(e => e.CourseId).ToHashSet();
        var records = (await progress.GetByUserAsync(user.Id, ct)).Where(p => active.Contains(p.CourseId)).ToList();
        var percent = records.Count == 0 ? 0 : (int)Math.Round(records.Average(p => (double)p.CompletionPercent));
        return Map(user, mine, active.Count, percent);
    }

    private async Task<BatchDto> GetBatchDtoAsync(Batch batch, CancellationToken ct) =>
        MapBatch(batch, await users.GetAllAsync(ct), (await courses.GetAllAsync(ct)).ToDictionary(c => c.Id));

    private static StudentDto Map(User u, List<Batch> mine, int activeCourses, int percent)
    {
        var first = mine.FirstOrDefault();
        return new StudentDto(
            u.Id, u.FirstName, u.LastName, u.FullName, u.Email, u.PhoneNumber, u.Status, u.EmailVerified, u.PreferredLanguage,
            first?.Id, first?.Name, u.AddedByStaff, u.MustChangePassword, u.CreatedAtUtc, u.LastLoginAtUtc,
            activeCourses, percent, u.IsDeleted,
            mine.Select(b => new StudentBatchDto(b.Id, b.Name, b.StartTime, b.EndTime)).ToList(),
            u.AwaitingLogin);
    }

    private static BatchDto MapBatch(Batch b, IReadOnlyList<User> allUsers, Dictionary<Guid, Course> allCourses) => new(
        b.Id,
        b.Name,
        b.Level,
        b.Description,
        b.MentorId,
        b.MentorId.HasValue ? allUsers.FirstOrDefault(u => u.Id == b.MentorId.Value)?.FullName : null,
        b.CourseIds.Where(allCourses.ContainsKey).Select(id => allCourses[id])
            .Where(c => !c.IsDeleted)
            .Select(c => new BatchCourseDto(c.Id, c.Title, c.CefrLevel, c.Status)).ToList(),
        b.StartDateUtc,
        allUsers.Count(u => u.Role == UserRole.User && u.IsInBatch(b.Id) && !u.IsDeleted),
        b.CreatedAtUtc,
        b.StartTime,
        b.EndTime);

    /// <summary>12 characters, easy to read aloud (no 0/O or 1/l), always with a digit and a symbol.</summary>
    public static string GeneratePassword()
    {
        const string letters = "ABCDEFGHJKMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz";
        const string digits = "23456789";
        var chars = new char[12];
        for (var i = 0; i < 9; i++) chars[i] = letters[RandomNumberGenerator.GetInt32(letters.Length)];
        chars[9] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        chars[10] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        chars[11] = '!';
        return new string(chars);
    }

    private Task LogAsync(string action, string details, CancellationToken ct) => audit.AddAsync(new AuditLog
    {
        ActorUserId = current.UserId ?? Guid.Empty,
        Action = action,
        Resource = "student",
        Details = details
    }, ct);

    private void EnsureCanView()
    {
        if (!current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Staff, UserRole.Mentor))
            throw new UnauthorizedAccessDomainException("Team access required.");
    }

    private void EnsureCanManageStudents()
    {
        if (!CanManageStudents)
            throw new UnauthorizedAccessDomainException("You don't have permission to change student accounts.");
    }

    private void EnsureAdmin()
    {
        if (!IsAdmin)
            throw new UnauthorizedAccessDomainException("You don't have permission to create or change batches.");
    }
}
