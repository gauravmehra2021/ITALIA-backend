using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SSeuropa.Application.Interfaces;

namespace SSeuropa.Infrastructure.Persistence;

public sealed class MongoDbInitializer(
    MongoDbContext db,
    MongoDbOptions options,
    IPasswordHasher hasher,
    IConfiguration configuration,
    ILogger<MongoDbInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.EnsureIndexesAsync(cancellationToken);

            if (options.SeedDemoDataOnEmpty)
            {
                await SeedIfEmptyAsync(cancellationToken);
            }

            try
            {
                await EnsureSuperAdminAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not ensure SuperAdmin on startup (transient connection reset).");
            }

            try
            {
                await EnsureDemoStudentAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not ensure DemoStudent on startup (transient connection reset).");
            }

            logger.LogInformation("MongoDB ready on database '{DatabaseName}'.", db.Database.DatabaseNamespace.DatabaseName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Warning initializing MongoDB on database '{DatabaseName}'. Verify ConnectionStrings:MongoDb.", db.Database.DatabaseNamespace.DatabaseName);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task SeedIfEmptyAsync(CancellationToken ct)
    {
        var userCount = await db.Users.CountDocumentsAsync(FilterDefinition<Domain.Entities.User>.Empty, cancellationToken: ct);
        if (userCount > 0)
        {
            return;
        }

        var seed = InMemoryStore.CreateSeeded(hasher.Hash);

        await InsertIfAnyAsync(db.Users, seed.Users, ct);
        await InsertIfAnyAsync(db.Courses, seed.Courses, ct);
        await InsertIfAnyAsync(db.Classes, seed.Classes, ct);
        await InsertIfAnyAsync(db.Enrollments, seed.Enrollments, ct);
        await InsertIfAnyAsync(db.Materials, seed.Materials, ct);
        await InsertIfAnyAsync(db.Assessments, seed.Assessments, ct);
        await InsertIfAnyAsync(db.Progress, seed.Progress, ct);
        await InsertIfAnyAsync(db.Subscriptions, seed.Subscriptions, ct);
        await InsertIfAnyAsync(db.Payments, seed.Payments, ct);
        await InsertIfAnyAsync(db.Certificates, seed.Certificates, ct);
        await InsertIfAnyAsync(db.Notifications, seed.Notifications, ct);
        await InsertIfAnyAsync(db.Announcements, seed.Announcements, ct);
        await InsertIfAnyAsync(db.Tickets, seed.Tickets, ct);
        await InsertIfAnyAsync(db.AuditLogs, seed.AuditLogs, ct);
        await InsertIfAnyAsync(db.Enquiries, seed.Enquiries, ct);

        logger.LogInformation("Seeded initial SS Europa collections in MongoDB.");
    }

    private async Task EnsureSuperAdminAsync(CancellationToken ct)
    {
        // 1. Check or create admin@sseuropa.com
        var adminPassword = configuration["Seed:SuperAdminPassword"]
            ?? configuration["SUPERADMIN_DEFAULT_PASSWORD"]
            ?? Environment.GetEnvironmentVariable("SUPERADMIN_DEFAULT_PASSWORD")
            ?? "Admin@SSEuropa2026!";

        var mainAdmin = await db.Users.Find(u => u.Email == "admin@sseuropa.com").FirstOrDefaultAsync(ct);
        if (mainAdmin is null)
        {
            mainAdmin = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Email = "admin@sseuropa.com",
                PasswordHash = hasher.Hash(adminPassword),
                FirstName = "SS Europa",
                LastName = "SuperAdmin",
                Role = Domain.Enums.UserRole.SuperAdmin,
                Status = Domain.Enums.AccountStatus.Active,
                EmailVerified = true,
                PreferredLanguage = Domain.Enums.PreferredLanguage.English,
                IsProtectedSuperAdmin = true,
                IsDeleted = false
            };
            await db.Users.InsertOneAsync(mainAdmin, cancellationToken: ct);
            logger.LogInformation("Root SuperAdmin 'admin@sseuropa.com' created successfully.");
        }
        else
        {
            mainAdmin.Role = Domain.Enums.UserRole.SuperAdmin;
            mainAdmin.IsProtectedSuperAdmin = true;
            mainAdmin.IsDeleted = false;
            mainAdmin.Status = Domain.Enums.AccountStatus.Active;
            await db.Users.ReplaceOneAsync(u => u.Id == mainAdmin.Id, mainAdmin, cancellationToken: ct);
        }

        // 2. There is exactly ONE Super Admin: admin@sseuropa.com.
        //    Any other account holding the role is downgraded to Admin. The seeded demo
        //    super admin (superadmin@sseuropa.demo, password in source) is also suspended.
        var otherSuperAdmins = await db.Users
            .Find(u => u.Id != mainAdmin.Id && (u.Role == Domain.Enums.UserRole.SuperAdmin || u.IsProtectedSuperAdmin))
            .ToListAsync(ct);
        foreach (var extra in otherSuperAdmins)
        {
            extra.Role = Domain.Enums.UserRole.Admin;
            extra.IsProtectedSuperAdmin = false;
            if (string.Equals(extra.Email, "superadmin@sseuropa.demo", StringComparison.OrdinalIgnoreCase))
            {
                extra.Status = Domain.Enums.AccountStatus.Suspended;
            }
            await db.Users.ReplaceOneAsync(u => u.Id == extra.Id, extra, cancellationToken: ct);
            logger.LogWarning(
                "Removed Super Admin role from {Email} (now Admin{Suspended}). admin@sseuropa.com is the only Super Admin.",
                extra.Email,
                extra.Status == Domain.Enums.AccountStatus.Suspended ? ", suspended" : string.Empty);
        }
    }

    private async Task EnsureDemoStudentAsync(CancellationToken ct)
    {
        var studentEmail = "student@sseuropa.demo";
        var demoStudent = await db.Users.Find(u => u.Email == studentEmail).FirstOrDefaultAsync(ct);
        if (demoStudent is null)
        {
            demoStudent = new Domain.Entities.User
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Email = studentEmail,
                PasswordHash = hasher.Hash("Demo@12345"),
                FirstName = "Demo",
                LastName = "Student",
                Role = Domain.Enums.UserRole.User,
                Status = Domain.Enums.AccountStatus.Active,
                EmailVerified = true,
                PreferredLanguage = Domain.Enums.PreferredLanguage.English,
                IsDeleted = false,
                UnlockAllLessons = true
            };
            await db.Users.InsertOneAsync(demoStudent, cancellationToken: ct);
            logger.LogInformation("Permanent Demo Student 'student@sseuropa.demo' created.");
        }
        else
        {
            demoStudent.PasswordHash = hasher.Hash("Demo@12345");
            demoStudent.Status = Domain.Enums.AccountStatus.Active;
            demoStudent.EmailVerified = true;
            demoStudent.IsDeleted = false;
            demoStudent.UnlockAllLessons = true; // the demo account reviews all content
            await db.Users.ReplaceOneAsync(u => u.Id == demoStudent.Id, demoStudent, cancellationToken: ct);
        }

        // Ensure demo student is enrolled in at least one course so mobile features are active immediately
        var firstCourse = await db.Courses.Find(FilterDefinition<Domain.Entities.Course>.Empty).FirstOrDefaultAsync(ct);
        if (firstCourse is not null)
        {
            var enrollment = await db.Enrollments.Find(e => e.UserId == demoStudent.Id && e.CourseId == firstCourse.Id).FirstOrDefaultAsync(ct);
            if (enrollment is null)
            {
                await db.Enrollments.InsertOneAsync(new Domain.Entities.Enrollment
                {
                    Id = Guid.NewGuid(),
                    UserId = demoStudent.Id,
                    CourseId = firstCourse.Id,
                    Status = Domain.Enums.EnrollmentStatus.Active,
                    ActivatedAtUtc = DateTime.UtcNow
                }, cancellationToken: ct);
            }
        }
    }

    private static async Task InsertIfAnyAsync<T>(IMongoCollection<T> collection, IReadOnlyCollection<T> items, CancellationToken ct)
    {
        if (items.Count == 0) return;
        var existing = await collection.CountDocumentsAsync(FilterDefinition<T>.Empty, cancellationToken: ct);
        if (existing == 0)
        {
            await collection.InsertManyAsync(items, cancellationToken: ct);
        }
    }
}


