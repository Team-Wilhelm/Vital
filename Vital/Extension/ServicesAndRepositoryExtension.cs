using Infrastructure.Initialize;
using Vital.Core.Services;
using Vital.Core.Services.Interfaces;

namespace Vital.Extension;

public static class ServicesAndRepositoryExtension
{
    public static IServiceCollection AddServicesAndRepositories(this IServiceCollection services)
    {
        #region Repository



        #endregion

        #region Service

        services.AddTransient<IJwtService, JwtService>();

        #endregion

        services.AddScoped<DbInitializer>();

        return services;
    }
}
