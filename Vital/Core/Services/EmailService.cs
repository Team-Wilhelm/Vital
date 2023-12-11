using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Models.Exception;
using Models.Identity;
using Vital.Configuration;
using Vital.Core.Services.Interfaces;

namespace Vital.Core.Services;

public class EmailService(IEmailDeliveryService emailDeliveryService, IOptions<GlobalSettings> globalSettings)
    : IEmailService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public async Task SendVerifyEmail(ApplicationUser user, string token, CancellationToken cancellationToken)
    {
        var recipients = new List<string>() { user.UserName! };
        var subject = "Verify Email";

        var verifyMessageContent = $@"
<body style=""font-family: Arial, sans-serif; background-color: #f0f0f0; margin: 0; padding: 0;"">
    <div style=""background-color: #ffffff; width: 100%; max-width: 600px; margin: 30px auto; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
        <div style=""background-color: #d1c1d7; color: black; padding: 10px; text-align: center; border-radius: 5px;"">
            <h1>Welcome to Vital!</h1>
        </div>
        <div style=""padding: 10px; text-align: center;"">
            <p>Hello there,</p>
            <p>Thanks for signing up for Vital, a women's health app dedicated to your well-being. Please confirm your email address to start your journey with us.</p>
            <a href=""{globalSettings.Value.FrontEndUrl}/verify-email?userId={user.Id}&token={HttpUtility.UrlEncode(token)}"" style=""display: inline-block; padding: 10px 20px; margin-top: 20px; background-color: #d1c1d7; color: black; text-decoration: none; border-radius: 5px;"">Verify Email</a>
            <p>If you didn't sign up for Vital, you can safely ignore this email.</p>
        </div>
    </div>
</body>
";
        var message = CreateDefaultMessage(verifyMessageContent);

        await SendEmailAsync(recipients, subject, message, cancellationToken);
    }

    public async Task SendForgotPasswordEmailAsync(ApplicationUser user, string token,
        CancellationToken cancellationToken)
    {
        var recipients = new List<string>() { user.UserName! };
        var subject = "Forgot password";

        var resetPasswordContent = $@"
<body style=""font-family: Arial, sans-serif; background-color: #f0f0f0; margin: 0; padding: 0;"">
<div style=""background-color: #ffffff; width: 100%; max-width: 600px; margin: 30px auto; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
    <div style=""background-color: #d1c1d7; color: black; padding: 10px; text-align: center; border-radius: 5px;"">
        <h1>Password Reset for Vital</h1>
    </div>
    <div style=""padding: 10px; text-align: center;"">
        <p>Hello there,</p>
        <p>We received a request to reset the password for your Vital account. Click the button below to set a new password:</p>
        <a href=""{globalSettings.Value.FrontEndUrl}/Forgot-Password?userId={user.Id}&token={HttpUtility.UrlEncode(token)}"" style=""display: inline-block; padding: 10px 20px; margin-top: 20px; background-color: #d1c1d7; color: black; text-decoration: none; border-radius: 5px;"">Reset Password</a>
        <p>If you did not request a password reset, please ignore this email.</p>
    </div>
</div>
</body>";

        var message = CreateDefaultMessage(resetPasswordContent);

        await SendEmailAsync(recipients, subject, message, cancellationToken);
    }

    #region Private methods

    private async Task SendEmailAsync(List<string> recipients, string subject, string message,
        CancellationToken ct = default)
    {
        var result = await emailDeliveryService.SendAsync(recipients, subject, message, ct);
        if (!result)
        {
            throw new EmailException("Could not send email");
        }
    }

    private string CreateDefaultMessage(string content)
    {
        var defaultTemplate = @"<html><body>{{PartialBlock}}<br>&copy; {{CurrentYear}} Vital</body></html>";

        return defaultTemplate.Replace("{{PartialBlock}}", content)
            .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());
    }

    #endregion
}
