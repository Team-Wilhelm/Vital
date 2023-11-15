using Infrastructure.Initialize;
using Infrastructure.Repository;
using Infrastructure.Repository.Interface;
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

        services.AddScoped<ICycleService, CycleService>();

        #endregion

        services.AddScoped<DbInitializer>();

        return services;
    }
}
