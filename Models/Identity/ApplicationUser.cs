using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Models.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid? CurrentCycleId { get; set; }

    [Range(0.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public float? CycleLength { get; set; } = 28;

    [Range(0.0, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
    public float? PeriodLength { get; set; } = 5;
    
    public DateTimeOffset? ResetPasswordTokenExpirationDate { get; set; } 
    public DateTimeOffset? VerifyEmailTokenExpirationDate { get; set; }
}
