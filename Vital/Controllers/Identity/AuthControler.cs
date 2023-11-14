using Vital.Core.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Models.Identity;
using Models.Responses;

namespace Vital.Controllers.Identity; 

[Route("/Identity/[controller]")]
public class AuthController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) {
            throw new Exception("Wrong username or password");
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            throw new Exception("Wrong username or password");
        }
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateJwtToken(user, roles, null);
        return Ok(new AuthResponse()
        {
            Email = user.Email!,
            UserId = user.Id,
            Token = token,
            Roles = roles.ToList()
        });
    }
}
