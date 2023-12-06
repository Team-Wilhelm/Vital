using Microsoft.Extensions.Options;
using Models.Identity;
using Vital.Configuration;
using Vital.Core.Services.Interfaces;
using Vital.Models.Exception;

namespace Vital.Core.Services;

public class EmailService(IEmailDeliveryService emailDeliveryService, IOptions<GlobalSettings> globalSettings) : IEmailService
{
    public async Task SendVerifyEmail(ApplicationUser user, string token, CancellationToken cancellationToken)
    {
        var recipients = new List<string>() { user.UserName! };
        var subject = "Verify Email";

        var verifyMessageContent = $@"
<p>
Hello, {user.UserName},
to verify your email click: <a style=""padding: 5px 15px; text-decoration:none; border-radius: 5px; background-color: #d1c1d7; color: #100e11"" href=""{globalSettings.Value.FrontEndUrl}/email-verify?userId={user.Id}&token={token}""> Verify email </a>  
</p> 
<p> 
If you did not request this email, please ignore it. 
</p>";

        var message = CreateDefaultMessage(verifyMessageContent);

        await SendEmailAsync(recipients, subject, message, cancellationToken);
    }
    
    public async Task SendForgotPasswordEmailAsync(ApplicationUser user, string token, CancellationToken cancellationToken)
    {
        var recipients = new List<string>() { user.UserName! };
        var subject = "Verify Email";

        var verifyMessageContent = $@"
<p>
Hello, {user.UserName}, 
to verify your email click: <a style=""""padding: 5px 15px; text-decoration:none; border-radius: 5px; background-color: #d1c1d7; color: #100e11"""" href=""""{globalSettings.Value.FrontEndUrl}/reset-password?userId={user.Id}&token={token}> Reset password </a>
</p>
<p>
If you did not request this email, please ignore it.
</p>";

        var message = CreateDefaultMessage(verifyMessageContent);

        await SendEmailAsync(recipients, subject, message, cancellationToken);
    }

    #region Private methods

    private async Task SendEmailAsync(List<string> recipients, string subject, string message, CancellationToken ct = default)
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
