using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto.Identity;
using Models.Identity;
using Models.Responses;
using Vital.Core.Services.Interfaces;

namespace Vital.Controllers.Identity;

[Route("/Identity/[controller]")]
public class AuthController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ICycleService _cycleService;
    private readonly IEmailService _emailService;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService, ICycleService cycleService, IEmailDeliveryService emailDeliveryService){
        _userManager = userManager;
        _jwtService = jwtService;
        _cycleService = cycleService;
        _emailService = emailService;
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("Wrong username or password");
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            throw new Exception("Wrong username or password");
        }

        if (!user.EmailConfirmed)
        {
            throw new Exception("Email is not confirmed");
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

    /// <summary>
    /// Register user
    /// </summary>
    /// <param name="requestDto"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        

        var user = new ApplicationUser()
        {
            Email = requestDto.Email,
            UserName = requestDto.Email
        };

        var result = await _userManager.CreateAsync(user, requestDto.Password);
        if (!result.Succeeded)
        {
            throw new Exception("Cannot create user");
        }

        return Ok();
    }
}
