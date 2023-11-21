namespace Vital.Core.Services.Interfaces;

public interface IMailService
{
    Task<bool> SendAsync(List<string> recipients, string subject, string message, CancellationToken ct = default);
}
