using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SSeuropa.Application.Interfaces;

namespace SSeuropa.Infrastructure.Persistence;

public sealed class MongoDbInitializer(
    MongoDbContext db,
    MongoDbOptions options,
    IPasswordHasher hasher,
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

            logger.LogInformation("MongoDB ready on database '{DatabaseName}'.", db.Database.DatabaseNamespace.DatabaseName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize MongoDB on database '{DatabaseName}'. Verify ConnectionStrings:MongoDb.", db.Database.DatabaseNamespace.DatabaseName);
            throw;
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

