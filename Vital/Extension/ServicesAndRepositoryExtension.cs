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

        #endregion

        #region Service

        services.AddTransient<ICycleService, CycleService>();
        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<IMailService, BrevoMailService>();

        #endregion

        services.AddScoped<CurrentContext>();

        services.AddScoped<DbInitializer>();

        return services;
    }
}
