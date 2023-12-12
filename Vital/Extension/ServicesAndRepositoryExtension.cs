using Infrastructure.Initialize;
using Infrastructure.Repository;
using Infrastructure.Repository.Interface;
using Vital.Core.Context;
using Vital.Core.Services;
using Vital.Core.Services.Interfaces;

namespace Vital.Extension;

public static class ServicesAndRepositoryExtension
{
    public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
    {
        #region Repository

        services.AddScoped<ICycleRepository, CycleRepository>();
        services.AddScoped<IMetricRepository, MetricRepository>();
        services.AddScoped<ICalendarDayRepository, CalendarDayRepository>();

        #endregion

        #region Service
        
        services.AddTransient<ICycleService, CycleService>();
        services.AddTransient<IJwtService, JwtService>();
        services.AddScoped<IMetricService, MetricService>();
        services.AddTransient<IEmailDeliveryService, BrevoEmailDeliveryService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddHostedService<UserCleanupService>();

        #endregion

        services.AddScoped<CurrentContext>();

        services.AddScoped<DbInitializer>();

        return services;
    }
}
