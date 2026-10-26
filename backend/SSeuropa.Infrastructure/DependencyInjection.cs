using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SSeuropa.Application.Interfaces;
using SSeuropa.Infrastructure.Auth;
using SSeuropa.Infrastructure.External;
using SSeuropa.Infrastructure.Persistence;
using SSeuropa.Infrastructure.Repositories;

namespace SSeuropa.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var hasher = new PasswordHasher();
        services.AddSingleton(InMemoryStore.CreateSeeded(hasher.Hash));
        services.AddSingleton<IPasswordHasher>(hasher);
        services.AddSingleton<IOtpService, OtpService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IStorageService, MockStorageService>();
        services.AddSingleton<IPaymentGateway, MockPaymentGateway>();
        services.AddSingleton<IAiLearningService, MockAiLearningService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<ISessionRepository, InMemorySessionRepository>();
        services.AddScoped<IOtpRepository, InMemoryOtpRepository>();
        services.AddScoped<ICourseRepository, InMemoryCourseRepository>();
        services.AddScoped<IClassRepository, InMemoryClassRepository>();
        services.AddScoped<IEnrollmentRepository, InMemoryEnrollmentRepository>();
        services.AddScoped<IMaterialRepository, InMemoryMaterialRepository>();
        services.AddScoped<IAssessmentRepository, InMemoryAssessmentRepository>();
        services.AddScoped<IAttendanceRepository, InMemoryAttendanceRepository>();
        services.AddScoped<IProgressRepository, InMemoryProgressRepository>();
        services.AddScoped<ISubscriptionRepository, InMemorySubscriptionRepository>();
        services.AddScoped<IPaymentRepository, InMemoryPaymentRepository>();
        services.AddScoped<ICertificateRepository, InMemoryCertificateRepository>();
        services.AddScoped<INotificationRepository, InMemoryNotificationRepository>();
        services.AddScoped<IAnnouncementRepository, InMemoryAnnouncementRepository>();
        services.AddScoped<ISupportTicketRepository, InMemorySupportTicketRepository>();
        services.AddScoped<IAuditLogRepository, InMemoryAuditLogRepository>();
        services.AddScoped<IAiSessionRepository, InMemoryAiSessionRepository>();
        services.AddScoped<IEnquiryRepository, InMemoryEnquiryRepository>();

        var key = configuration["Jwt:Key"] ?? "SSEuropa-Demo-Signing-Key-At-Least-32-Chars!";
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "SSeuropa",
                    ValidAudience = configuration["Jwt:Audience"] ?? "SSeuropa.Web",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();
        return services;
    }
}
