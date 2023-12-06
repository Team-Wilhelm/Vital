using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dto.Identity;
using Models.Identity;
using Models.Responses;
using Vital.Core.Services.Interfaces;
using Vital.Models.Exception;

namespace Vital.Controllers.Identity;

[Route("/Identity/[controller]")]
public class AuthController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly ICycleService _cycleService;
    private readonly IEmailService _emailService;

    public AuthController(UserManager<ApplicationUser> userManager, IJwtService jwtService, ICycleService cycleService, IEmailService emailService)
    {
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.Email);
        if (user == null)
        {
            throw new AuthException("Wrong username or password");
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            throw new AuthException("Wrong username or password");
        }

        if (!user.EmailConfirmed)
        {
            throw new AuthException("Email is not confirmed");
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
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            Email = requestDto.Email,
            UserName = requestDto.Email,
            CycleLength = null,
            PeriodLength = null
        };
        var result = new IdentityResult();
        try
        {
            result = await _userManager.CreateAsync(user, requestDto.Password);
        } catch (DbUpdateException e)
        {
            throw new AuthException("Cannot create user. This username is already taken.");
        }
        if (!result.Succeeded)
        {
            var identityError = result.Errors?.FirstOrDefault();

            if (identityError == null)
            {
                throw new AuthException("Unknown error occurred while creating user.");
            }

            if (identityError.Code == "DuplicateUserName")
            {
                throw new AuthException("Cannot create user. This username is already taken.");
            }
            throw new AuthException(identityError.Description);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendVerifyEmail(user, token, cancellationToken);

        return Ok();
    }
    
    [HttpGet("username-taken/{username}")]
    public async Task<IActionResult> IsUsernameTaken([FromRoute] string username)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        return Ok(user != null);
    }
}
