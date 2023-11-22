using Models.Identity;

namespace Vital.Core.Services.Interfaces;

public interface IEmailService
{
    Task SendVerifyEmail(ApplicationUser user, string token, CancellationToken cancellationToken);
}
