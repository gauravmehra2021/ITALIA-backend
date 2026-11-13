using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;

namespace SSeuropa.Infrastructure.Persistence;

public sealed class MongoHealthCheck(MongoDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await db.Database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy($"MongoDB connected ({db.Database.DatabaseNamespace.DatabaseName}).");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB connection failed.", ex);
        }
    }
}

