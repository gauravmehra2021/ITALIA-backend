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
/// A student may join the courses assigned to their batch plus any course marked "open to all"
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
        if (user.BatchId is { } batchId && await batches.GetByIdAsync(batchId, ct) is { } batch)
            allowed.UnionWith(batch.CourseIds);
        return allowed;
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
        var batchCourses = user.BatchId is { } batchId && await batches.GetByIdAsync(batchId, ct) is { } batch
            ? batch.CourseIds.Distinct().ToList()
            : [];
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
    private bool IsAdmin => current.IsInRole(UserRole.Admin, UserRole.SuperAdmin);
    private bool CanManageStudents => current.IsInRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Staff);
    private bool IsTeacher => current.Role == UserRole.Mentor;

    // ══════════════════════════════════════════════════════════════════════
    //  Students
    // ══════════════════════════════════════════════════════════════════════

    public async Task<IReadOnlyList<StudentDto>> GetStudentsAsync(CancellationToken ct = default)
    {
        EnsureCanView();
        var allBatches = (await batches.GetAllAsync(ct)).ToDictionary(b => b.Id);
        var students = (await users.GetAllAsync(ct)).Where(u => u.Role == UserRole.User);

        if (IsTeacher)
        {
            var mine = allBatches.Values.Where(b => b.MentorId == current.UserId).Select(b => b.Id).ToHashSet();
            students = students.Where(u => u.BatchId.HasValue && mine.Contains(u.BatchId.Value) && !u.IsDeleted);
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
                var batch = u.BatchId.HasValue ? allBatches.GetValueOrDefault(u.BatchId.Value) : null;
                return Map(u, batch, active.Count, percent);
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

        var batch = await GetBatchOrNullAsync(request.BatchId, ct);
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
            MustChangePassword = true,
            BatchId = batch?.Id
        };
        await users.AddAsync(user, ct);
        await access.SyncEnrollmentsAsync(user, ct);

        await email.SendStudentWelcomeAsync(user.Email, user.FirstName, password, batch?.Name, isReset: false, ct);
        await LogAsync("student_created", $"Created student {user.Email}{(batch is null ? "" : $" in batch '{batch.Name}'")}", ct);

        return new StudentPasswordResultDto(await GetDtoAsync(user, ct), password);
    }

    public async Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentRequest request, CancellationToken ct = default)
    {
        EnsureCanManageStudents();
        var user = await GetStudentAsync(id, ct);
        if (string.IsNullOrWhiteSpace(request.FirstName))
            throw new ValidationDomainException("First name is required.");

        var batch = await GetBatchOrNullAsync(request.BatchId, ct);
        var batchChanged = user.BatchId != batch?.Id;

        user.FirstName = request.FirstName.Trim();
        user.LastName = (request.LastName ?? "").Trim();
        user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        if (request.PreferredLanguage.HasValue) user.PreferredLanguage = request.PreferredLanguage.Value;
        user.BatchId = batch?.Id;
        await users.UpdateAsync(user, ct);
        if (batchChanged) await access.SyncEnrollmentsAsync(user, ct);

        await LogAsync("student_updated", $"Updated student {user.Email}{(batchChanged ? $" → batch '{batch?.Name ?? "none"}'" : "")}", ct);
        return await GetDtoAsync(user, ct);
    }

    /// <summary>Places several students in one batch (or takes them out of their batch when BatchId is null).</summary>
    public async Task<MoveStudentsResultDto> MoveStudentsAsync(MoveStudentsRequest request, CancellationToken ct = default)
    {
        EnsureCanManageStudents();
        if (request.StudentIds is null || request.StudentIds.Count == 0)
            throw new ValidationDomainException("Choose at least one student.");

        var batch = await GetBatchOrNullAsync(request.BatchId, ct);
        var moved = 0;
        foreach (var id in request.StudentIds.Distinct())
        {
            var user = await GetStudentAsync(id, ct);
            if (user.BatchId == batch?.Id) continue;
            user.BatchId = batch?.Id;
            await users.UpdateAsync(user, ct);
            await access.SyncEnrollmentsAsync(user, ct);
            moved++;
        }

        await LogAsync("students_moved", $"Moved {moved} student(s) to batch '{batch?.Name ?? "none"}'", ct);
        return new MoveStudentsResultDto(moved, batch?.Name);
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
        var password = GeneratePassword();
        user.PasswordHash = hasher.Hash(password);
        user.MustChangePassword = true;
        await users.UpdateAsync(user, ct);

        var batch = await GetBatchOrNullAsync(user.BatchId, ct, throwIfMissing: false);
        await email.SendStudentWelcomeAsync(user.Email, user.FirstName, password, batch?.Name, isReset: true, ct);
        await LogAsync("student_password_reset", $"Reset password for student {user.Email}", ct);
        return new StudentPasswordResultDto(await GetDtoAsync(user, ct), password);
    }

    // ══════════════════════════════════════════════════════════════════════
    //  Batches
    // ══════════════════════════════════════════════════════════════════════

    public async Task<IReadOnlyList<BatchDto>> GetBatchesAsync(CancellationToken ct = default)
    {
        EnsureCanView();
        var list = await batches.GetAllAsync(ct);
        if (IsTeacher) list = list.Where(b => b.MentorId == current.UserId).ToList();

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
            throw new ValidationDomainException($"This batch still has {students.Count} student(s). Move them to another batch first.");
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
        batch.Level = string.IsNullOrWhiteSpace(request.Level) ? "A0" : request.Level.Trim().ToUpperInvariant();
        batch.Description = (request.Description ?? "").Trim();
        batch.MentorId = request.MentorId;
        batch.CourseIds = courseIds;
        batch.StartDateUtc = request.StartDateUtc;
    }

    private async Task<List<User>> StudentsInAsync(Guid batchId, CancellationToken ct) =>
        (await users.GetAllAsync(ct)).Where(u => u.Role == UserRole.User && u.BatchId == batchId && !u.IsDeleted).ToList();

    private async Task<Batch?> GetBatchOrNullAsync(Guid? batchId, CancellationToken ct, bool throwIfMissing = true)
    {
        if (batchId is null) return null;
        var batch = await batches.GetByIdAsync(batchId.Value, ct);
        if (batch is null && throwIfMissing) throw new ValidationDomainException("That batch no longer exists.");
        return batch;
    }

    private async Task<User> GetStudentAsync(Guid id, CancellationToken ct)
    {
        var user = await users.GetByIdAsync(id, ct) ?? throw new NotFoundException("Student", id);
        if (user.Role != UserRole.User) throw new ValidationDomainException("This account is a team member, not a student.");
        return user;
    }

    private async Task<StudentDto> GetDtoAsync(User user, CancellationToken ct)
    {
        var batch = await GetBatchOrNullAsync(user.BatchId, ct, throwIfMissing: false);
        var active = (await enrollments.GetByUserAsync(user.Id, ct)).Where(e => e.Status == EnrollmentStatus.Active).Select(e => e.CourseId).ToHashSet();
        var records = (await progress.GetByUserAsync(user.Id, ct)).Where(p => active.Contains(p.CourseId)).ToList();
        var percent = records.Count == 0 ? 0 : (int)Math.Round(records.Average(p => (double)p.CompletionPercent));
        return Map(user, batch, active.Count, percent);
    }

    private async Task<BatchDto> GetBatchDtoAsync(Batch batch, CancellationToken ct) =>
        MapBatch(batch, await users.GetAllAsync(ct), (await courses.GetAllAsync(ct)).ToDictionary(c => c.Id));

    private static StudentDto Map(User u, Batch? batch, int activeCourses, int percent) => new(
        u.Id, u.FirstName, u.LastName, u.FullName, u.Email, u.PhoneNumber, u.Status, u.EmailVerified, u.PreferredLanguage,
        batch?.Id, batch?.Name, u.AddedByStaff, u.MustChangePassword, u.CreatedAtUtc, u.LastLoginAtUtc,
        activeCourses, percent, u.IsDeleted);

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
        allUsers.Count(u => u.Role == UserRole.User && u.BatchId == b.Id && !u.IsDeleted),
        b.CreatedAtUtc);

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
            throw new UnauthorizedAccessDomainException("Only Admin and Staff can change student accounts.");
    }

    private void EnsureAdmin()
    {
        if (!IsAdmin)
            throw new UnauthorizedAccessDomainException("Only an Admin can create or change batches.");
    }
}
