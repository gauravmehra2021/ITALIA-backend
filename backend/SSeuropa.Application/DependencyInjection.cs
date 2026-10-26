using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SSeuropa.Application.Services;

namespace SSeuropa.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Validation.RegisterRequestValidator>();
        services.AddScoped<AuthService>();
        services.AddScoped<CourseService>();
        services.AddScoped<ClassService>();
        services.AddScoped<MaterialService>();
        services.AddScoped<AssessmentService>();
        services.AddScoped<ProgressService>();
        services.AddScoped<SubscriptionService>();
        services.AddScoped<CertificateService>();
        services.AddScoped<AiService>();
        services.AddScoped<CommunicationService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<EnquiryService>();
        return services;
    }
}
