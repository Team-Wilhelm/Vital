using Models.Identity;

namespace Vital.Core.Services.Interfaces; 

public interface IJwtService
{
    string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles,
        IDictionary<string, dynamic>? customClaims);
}
