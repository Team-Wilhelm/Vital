namespace Vital.Core.Services.Interfaces;

public interface IEmailDeliveryService
{
    Task<bool> SendAsync(List<string> recipients, string subject, string message, CancellationToken ct = default);
}
