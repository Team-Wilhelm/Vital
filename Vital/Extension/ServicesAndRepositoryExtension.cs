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

        services.AddScoped<ICycleService, CycleService>();
        services.AddTransient<IJwtService, JwtService>();
        services.AddScoped<IMetricService, MetricService>();

        #endregion

        services.AddScoped<CurrentContext>();

        services.AddScoped<DbInitializer>();

        return services;
    }
}
