using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vital.Core.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Identity;
using Vital.Configuration;

namespace Vital.Core.Services; 

public class JwtService : IJwtService
{
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly ILogger<JwtService> _logger;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        ILogger<JwtService> logger)
    {
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    public string GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles, IDictionary<string, dynamic>? customClaims)
    {
        var claims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        };
        if (customClaims is not null)
        {
            claims.AddRange(customClaims.Select(c => new Claim(c.Key, c.Value)));
        }

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.Now.AddMinutes(_jwtSettings.Value.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Value.Issuer,
            audience: _jwtSettings.Value.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
