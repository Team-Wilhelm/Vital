using System.ComponentModel.DataAnnotations;

namespace Models.Dto.Identity;

public class RegisterRequestDto
{
    /// <summary>
    /// The user's email address which acts as a user name.
    /// </summary>
    [EmailAddress]
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }
    
    /// <summary>
    /// Average length of the user's cycle.
    /// </summary>
    public required int CycleLength { get; init; }
    
    /// <summary>
    /// Average length of the user's period.
    /// </summary>
    public required int PeriodLength { get; init; }
    
    public required DateTimeOffset LastPeriodStart { get; init; }
    
    /// <summary>
    /// This is optional, because the user can register during their period.
    /// </summary>
    public DateTimeOffset? LastPeriodEnd { get; init; } 
}
