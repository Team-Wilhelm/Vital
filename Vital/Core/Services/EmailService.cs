﻿using Microsoft.Extensions.Options;
using Models.Identity;
using Vital.Configuration;
using Vital.Core.Services.Interfaces;

namespace Vital.Core.Services;

public class EmailService(IEmailDeliveryService emailDeliveryService, IOptions<GlobalSettings> globalSettings) : IEmailService
{
    public async Task SendVerifyEmail(ApplicationUser user, string token, CancellationToken cancellationToken)
    {
        var recipients = new List<string>() { user.UserName! };
        var subject = "Verify Email";

        var verifyMessageContent = $"Hello, {user.UserName}, to verify your email click: {globalSettings.Value.FrontEndUrl}/email-verify?userId={user.Id}&token={token} <br/> If you did not request this email, please ignore it.";

        var message = CreateDefaultMessage(verifyMessageContent);

        await SendEmailAsync(recipients, subject, message, cancellationToken);
    }
    
    public async Task SendForgotPasswordEmailAsync(ApplicationUser user, string token, CancellationToken cancellationToken)
    {
        var recipients = new List<string>() { user.UserName! };
        var subject = "Verify Email";

        var verifyMessageContent = $"Hello, {user.UserName}, to verify your email click: {globalSettings.Value.FrontEndUrl}/reset-password?userId={user.Id}&token={token} <br/> If you did not request this email, please ignore it.";

        var message = CreateDefaultMessage(verifyMessageContent);

        await SendEmailAsync(recipients, subject, message, cancellationToken);
    }

    #region Private methods

    private async Task SendEmailAsync(List<string> recipients, string subject, string message, CancellationToken ct = default)
    {
        await emailDeliveryService.SendAsync(recipients, subject, message, ct);
    }

    private string CreateDefaultMessage(string content)
    {
        var defaultTemplate = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <meta name=\"viewport\" content=\"width=device-width\" />\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\r\n    <title>Vital</title>\r\n</head>\r\n\r\n<body style=\"-webkit-font-smoothing: antialiased; -webkit-text-size-adjust: none; height: 100%; line-height: 25px; width: 100% !important; margin: 0;\" bgcolor=\"#f6f6f6\">\r\n<style type=\"text/css\">\r\n    body {\r\n        margin: 0;\r\n        font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif;\r\n        box-sizing: border-box;\r\n        font-size: 16px;\r\n        color: #333;\r\n        line-height: 25px;\r\n        -webkit-font-smoothing: antialiased;\r\n        -webkit-text-size-adjust: none;\r\n    }\r\n\r\n    body * {\r\n        margin: 0;\r\n        font-family: \"Helvetica Neue\", Helvetica, Arial, sans-serif;\r\n        box-sizing: border-box;\r\n        font-size: 16px;\r\n        color: #333;\r\n        line-height: 25px;\r\n        -webkit-font-smoothing: antialiased;\r\n        -webkit-text-size-adjust: none;\r\n    }\r\n\r\n    img {\r\n        max-width: 100%;\r\n        border: none;\r\n    }\r\n\r\n    body {\r\n        -webkit-font-smoothing: antialiased;\r\n        -webkit-text-size-adjust: none;\r\n        width: 100% !important;\r\n        height: 100%;\r\n        line-height: 25px;\r\n    }\r\n\r\n    body {\r\n        background-color: #f6f6f6;\r\n    }\r\n\r\n    @media only screen and (max-width: 600px) {\r\n        body {\r\n            padding: 0 !important;\r\n        }\r\n\r\n        .container {\r\n            padding: 0 !important;\r\n            width: 100% !important;\r\n        }\r\n\r\n        .container-table {\r\n            padding: 0 !important;\r\n            width: 100% !important;\r\n        }\r\n\r\n        .content {\r\n            padding: 0 0 10px 0 !important;\r\n        }\r\n\r\n        .content-wrap {\r\n            padding: 10px !important;\r\n        }\r\n\r\n        .invoice {\r\n            width: 100% !important;\r\n        }\r\n\r\n        .main {\r\n            border-right: none !important;\r\n            border-left: none !important;\r\n            border-radius: 0 !important;\r\n        }\r\n\r\n        .logo {\r\n            padding-top: 10px !important;\r\n        }\r\n\r\n        .footer {\r\n            margin-top: 10px !important;\r\n        }\r\n\r\n        .indented {\r\n            padding-left: 10px;\r\n        }\r\n    }\r\n\r\n    @media only screen and (min-width: 600px) {\r\n        {{! Fix for Apple Mail }}\r\n        .content-table {\r\n            width: 600px !important;\r\n        }\r\n    }\r\n</style>\r\n<!--{{! Yahoo center fix }}-->\r\n<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" bgcolor=\"#f6f6f6\"><tr><td class=\"container\" width=\"100%\" align=\"center\">\r\n<!--    {{! 600px container }}-->\r\n    <table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"content-table\">\r\n        <tr>\r\n            <td></td> \r\n<!--            {{! Left column (center fix) }}-->\r\n            <td class=\"content\" align=\"center\" valign=\"top\" width=\"600\" style=\"padding-bottom: 20px;\">\r\n                <table class=\"header\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">\r\n                    <tr>\r\n                        <td valign=\"middle\" class=\"aligncenter middle logo\" style=\"padding: 20px 0 10px;\" align=\"center\">\r\n                            <img src=\"https://freemadic.com/wp-content/uploads/2014/12/place-holder.jpg\" alt=\"\" width=\"250\" height=\"39\" />\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n                <table class=\"main\" cellpadding=\"0\" cellspacing=\"0\" style=\"border: 1px solid #e9e9e9; border-radius: 3px;\" bgcolor=\"white\">\r\n                    <tr>\r\n                        <td class=\"content-wrap\" style=\"font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; box-sizing: border-box; font-size: 16px; color: #333; line-height: 25px; margin: 0; -webkit-font-smoothing: antialiased; padding: 20px; -webkit-text-size-adjust: none;\" valign=\"top\">\r\n\r\n                            {{PartialBlock}}\r\n\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n                <table class=\"footer\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"margin: 0; width: 100%;\">\r\n<!--                    <tr>-->\r\n<!--                        <td class=\"aligncenter social-icons\" align=\"center\" style=\"margin: 0; padding: 15px 0 0 0;\" valign=\"top\">-->\r\n<!--                            <table cellpadding=\"0\" cellspacing=\"0\" style=\"margin: 0 auto;\">-->\r\n<!--                                <tr>-->\r\n<!--                                    <td style=\"margin: 0; padding: 0 10px;\" valign=\"top\"><a href=\"https://twitter.com\" target=\"_blank\"><img src=\"\" alt=\"Twitter\" width=\"30\" height=\"30\" /></a></td>-->\r\n<!--                                    <td style=\"margin: 0; padding: 0 10px;\" valign=\"top\"><a href=\"https://www.youtube.com target=\"_blank\"><img src=\"\" alt=\"Youtube\" width=\"30\" height=\"30\" /></a></td>-->\r\n<!--                                    <td style=\"margin: 0; padding: 0 10px;\" valign=\"top\"><a href=\"https://www.linkedin.com\" target=\"_blank\"><img src=\"\" alt=\"LinkedIn\" width=\"30\" height=\"30\" /></a></td>-->\r\n<!--                                    <td style=\"margin: 0; padding: 0 10px;\" valign=\"top\"><a href=\"https://www.facebook.com\" target=\"_blank\"><img src=\"\" alt=\"Facebook\" width=\"30\" height=\"30\" /></a></td>-->\r\n<!--                                </tr>-->\r\n<!--                            </table>-->\r\n<!--                        </td>-->\r\n<!--                    </tr>-->\r\n                    <tr>\r\n                        <td class=\"content-block\" style=\"font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; box-sizing: border-box; font-size: 14px; color: #666666; line-height: 25px; margin: 0; -webkit-font-smoothing: antialiased; padding: 15px 0 0 0; -webkit-text-size-adjust: none; text-align: center;\" valign=\"top\">\r\n                            &copy; {{CurrentYear}} Vital\r\n                        </td>\r\n                    </tr>\r\n                </table>\r\n            </td>\r\n            <td></td> \r\n<!--            {{! Right column (center fix) }}-->\r\n        </tr>\r\n    </table>\r\n</td></tr></table>\r\n</body>\r\n</html>\r\n";

        return defaultTemplate.Replace("{{PartialBlock}}", content)
            .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());
    }

    #endregion
}