using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Models.Identity;

namespace Vital.Extension;

public static class IdentityBuilder
{
    public static IServiceCollection SetupIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            // Sign in settings.
            options.SignIn.RequireConfirmedEmail = true;
            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            
            options.Stores.ProtectPersonalData = true;
        })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddPersonalDataProtection<CustomLookupProtector, CustomLookupProtectorKeyRing>();

        return services;
    }
}
