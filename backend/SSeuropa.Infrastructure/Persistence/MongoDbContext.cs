using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using SSeuropa.Domain.Entities;

namespace SSeuropa.Infrastructure.Persistence;

public sealed class MongoDbOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "sseuropa";
    public bool SeedDemoDataOnEmpty { get; set; } = true;
}

public sealed class MongoDbContext
{
    private static int _conventionsRegistered;

    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }

    public MongoDbContext(MongoDbOptions options)
    {
        EnsureBsonConventions();

        var mongoUrl = MongoUrl.Create(options.ConnectionString);
        var settings = MongoClientSettings.FromUrl(mongoUrl);
        if (settings.UseTls)
        {
            settings.SslSettings ??= new SslSettings();
            settings.SslSettings.CheckCertificateRevocation = false;
        }

        var dbName = !string.IsNullOrWhiteSpace(mongoUrl.DatabaseName)
            ? mongoUrl.DatabaseName
            : options.DatabaseName;

        Client = new MongoClient(settings);
        Database = Client.GetDatabase(dbName);
    }

    public IMongoCollection<User> Users => Database.GetCollection<User>("users");
    public IMongoCollection<UserSession> Sessions => Database.GetCollection<UserSession>("user_sessions");
    public IMongoCollection<OtpChallenge> Otps => Database.GetCollection<OtpChallenge>("otp_challenges");
    public IMongoCollection<Course> Courses => Database.GetCollection<Course>("courses");
    public IMongoCollection<ClassSession> Classes => Database.GetCollection<ClassSession>("class_sessions");
    public IMongoCollection<Enrollment> Enrollments => Database.GetCollection<Enrollment>("enrollments");
    public IMongoCollection<Material> Materials => Database.GetCollection<Material>("materials");
    public IMongoCollection<Assessment> Assessments => Database.GetCollection<Assessment>("assessments");
    public IMongoCollection<TestAttempt> Attempts => Database.GetCollection<TestAttempt>("test_attempts");
    public IMongoCollection<AttendanceRecord> Attendance => Database.GetCollection<AttendanceRecord>("attendance_records");
    public IMongoCollection<LearningProgress> Progress => Database.GetCollection<LearningProgress>("learning_progress");
    public IMongoCollection<Subscription> Subscriptions => Database.GetCollection<Subscription>("subscriptions");
    public IMongoCollection<Payment> Payments => Database.GetCollection<Payment>("payments");
    public IMongoCollection<Certificate> Certificates => Database.GetCollection<Certificate>("certificates");
    public IMongoCollection<Notification> Notifications => Database.GetCollection<Notification>("notifications");
    public IMongoCollection<Announcement> Announcements => Database.GetCollection<Announcement>("announcements");
    public IMongoCollection<SupportTicket> Tickets => Database.GetCollection<SupportTicket>("support_tickets");
    public IMongoCollection<AuditLog> AuditLogs => Database.GetCollection<AuditLog>("audit_logs");
    public IMongoCollection<AiSession> AiSessions => Database.GetCollection<AiSession>("ai_sessions");
    public IMongoCollection<Enquiry> Enquiries => Database.GetCollection<Enquiry>("enquiries");

    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        await Users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true }),
            cancellationToken: ct);

        await Sessions.Indexes.CreateManyAsync([
            new CreateIndexModel<UserSession>(Builders<UserSession>.IndexKeys.Ascending(s => s.RefreshToken)),
            new CreateIndexModel<UserSession>(Builders<UserSession>.IndexKeys.Ascending(s => s.UserId).Ascending(s => s.IsRevoked))
        ], ct);

        await Otps.Indexes.CreateOneAsync(
            new CreateIndexModel<OtpChallenge>(
                Builders<OtpChallenge>.IndexKeys
                    .Ascending(o => o.UserId)
                    .Ascending(o => o.Purpose)
                    .Ascending(o => o.IsUsed)),
            cancellationToken: ct);

        await Enrollments.Indexes.CreateOneAsync(
            new CreateIndexModel<Enrollment>(
                Builders<Enrollment>.IndexKeys.Ascending(e => e.UserId).Ascending(e => e.CourseId),
                new CreateIndexOptions { Unique = true }),
            cancellationToken: ct);

        await Attendance.Indexes.CreateOneAsync(
            new CreateIndexModel<AttendanceRecord>(
                Builders<AttendanceRecord>.IndexKeys.Ascending(a => a.ClassSessionId).Ascending(a => a.UserId),
                new CreateIndexOptions { Unique = true }),
            cancellationToken: ct);

        await Progress.Indexes.CreateOneAsync(
            new CreateIndexModel<LearningProgress>(
                Builders<LearningProgress>.IndexKeys.Ascending(p => p.UserId).Ascending(p => p.CourseId),
                new CreateIndexOptions { Unique = true }),
            cancellationToken: ct);

        await Certificates.Indexes.CreateOneAsync(
            new CreateIndexModel<Certificate>(
                Builders<Certificate>.IndexKeys.Ascending(c => c.CertificateCode),
                new CreateIndexOptions { Unique = true }),
            cancellationToken: ct);

        await Enquiries.Indexes.CreateManyAsync([
            new CreateIndexModel<Enquiry>(Builders<Enquiry>.IndexKeys.Descending(e => e.CreatedAtUtc)),
            new CreateIndexModel<Enquiry>(Builders<Enquiry>.IndexKeys.Ascending(e => e.Status).Descending(e => e.CreatedAtUtc))
        ], ct);

        await Notifications.Indexes.CreateOneAsync(
            new CreateIndexModel<Notification>(
                Builders<Notification>.IndexKeys.Ascending(n => n.UserId).Descending(n => n.CreatedAtUtc)),
            cancellationToken: ct);

        await AuditLogs.Indexes.CreateOneAsync(
            new CreateIndexModel<AuditLog>(Builders<AuditLog>.IndexKeys.Descending(a => a.CreatedAtUtc)),
            cancellationToken: ct);
    }

    private static void EnsureBsonConventions()
    {
        if (Interlocked.Exchange(ref _conventionsRegistered, 1) == 1)
        {
            return;
        }

        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var pack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String),
            new IgnoreExtraElementsConvention(true)
        };
        ConventionRegistry.Register("SSeuropaConventions", pack, _ => true);
    }
}

