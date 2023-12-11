using System.Net.Mime;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Dto.Identity.Account;
using Models.Exception;
using Models.Identity;
using Vital.Core.Context;
using Vital.Core.Services.Interfaces;
using Vital.Models.Exception;

namespace Vital.Controllers;

[Route("/Identity/[controller]")]
public class AccountController : BaseController
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly CurrentContext _currentContext;

    public AccountController(UserManager<ApplicationUser> userManager, IEmailService emailService, CurrentContext currentContext)
    {
        _userManager = userManager;
        _emailService = emailService;
        _currentContext = currentContext;
    }

    /// <summary>
    /// Request forgot password
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("Forgot-Password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user is not null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            user.ResetPasswordTokenExpirationDate = DateTime.UtcNow.AddHours(24);
            await _userManager.UpdateAsync(user);
            await _emailService.SendForgotPasswordEmailAsync(user, token, cancellationToken);
        }

        return Ok();
    }

    /// <summary>
    /// Reset user password
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("Reset-Password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

        if (user is null)
        {
            throw new ResetPasswordException("Something went wrong.");
        }

        if (user.ResetPasswordTokenExpirationDate is null || user.ResetPasswordTokenExpirationDate.HasValue && user.ResetPasswordTokenExpirationDate.Value < DateTime.UtcNow)
        {
            throw new ResetPasswordException("Link has expired.");
        }

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

        if (result.Succeeded)
        {
            user.ResetPasswordTokenExpirationDate = null;
        }

        return Ok();
    }

    /// <summary>
    /// Enables a logged in user to change their password as long as they know their existing password
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ResetPasswordException"></exception>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByIdAsync(_currentContext.UserId!.Value.ToString());

        if (user is null)
        {
            throw new ResetPasswordException("Something went wrong.");
        }

        if(await _userManager.CheckPasswordAsync(user, dto.OldPassword))
        {
            await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
        }

        return Ok();
    }

    /// <summary>
    /// Verify users email
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <response code="200">If verification was successful</response>
    /// <response code="400">If the verification failed</response>
    [HttpPost("Verify-Email")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyRequestDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

        if (user is null)
        {
            throw new EmailVerifyException("User not found.");
        }

        if (user.VerifyEmailTokenExpirationDate.HasValue && user.VerifyEmailTokenExpirationDate.Value < DateTime.UtcNow)
        {
            throw new EmailVerifyException("Link has expired.");
        }

        if (user.EmailConfirmed)
        {
            throw new EmailVerifyException("Email already verified.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, dto.Token);

        if (!result.Succeeded) throw new EmailVerifyException("Something went wrong.");
        user.VerifyEmailTokenExpirationDate = null;
        await _userManager.UpdateAsync(user);
        return Ok();
    }

    /// <summary>
    /// Retrieve email of logged in user
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotFoundException"></exception>
    [HttpGet("email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEmail()
    {
        var user = await _userManager.FindByIdAsync(_currentContext.UserId!.Value.ToString());
        if (user is null)
        {
            throw new NotFoundException("No user found.");
        }

        return Ok(new { user.Email });
    }
}
