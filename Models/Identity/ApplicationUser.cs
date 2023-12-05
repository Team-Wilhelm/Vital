using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Models.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid? CurrentCycleId { get; set; }
    
    [MinLength(1)]
    public float CycleLength { get; set; } = 28;
    
    [MinLength(1)]
    public float PeriodLength { get; set; } = 5;
}
