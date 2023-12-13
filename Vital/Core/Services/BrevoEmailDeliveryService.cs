﻿using Microsoft.Extensions.Options;
using Models.Exception;
using SendWithBrevo;
using Vital.Configuration;
using Vital.Core.Services.Interfaces;

namespace Vital.Core.Services;

public class BrevoEmailDeliveryService : IEmailDeliveryService
{
    private readonly BrevoSettings _brevoSettings;

    public BrevoEmailDeliveryService(IOptions<BrevoSettings> brevoSettings)
    {
        _brevoSettings = brevoSettings.Value;
    }

    public async Task<bool> SendAsync(List<string> recipients, string subject, string message, CancellationToken ct = default)
    {

        var brevoRecipients = recipients.Select(recipient => new Recipient(recipient.Split("@")[0], recipient)).ToList();

        if (_brevoSettings.ApiKey is null or "")
        {
            throw new BrevoException("API key is not set.");
        }
        var client = new BrevoClient(_brevoSettings.ApiKey);

        return await client.SendAsync(
            new Sender(_brevoSettings.DisplayName, _brevoSettings.From),
            brevoRecipients,
            subject,
            message,
            token: ct
        );
    }
}
