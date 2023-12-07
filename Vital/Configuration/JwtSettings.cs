namespace Vital.Configuration;

public class JwtSettings
{
    /// <summary>
    /// JWT key
    /// </summary>
    public string Key { get; set; } = null!;

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// JWT audience
    /// </summary>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// JWT expiration in minutes
    /// </summary>
    public int ExpirationMinutes { get; set; }
}
