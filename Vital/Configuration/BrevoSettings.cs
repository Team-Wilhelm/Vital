namespace Vital.Configuration;

public class BrevoSettings
{
    /// <summary>
    /// Display name of the sender
    /// </summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// Email address of the sender
    /// </summary>
    public string From { get; set; } = null!;

    /// <summary>
    /// API key for Brevo
    /// </summary>
    public string ApiKey { get; set; } = null!;
}
