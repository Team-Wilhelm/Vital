using Microsoft.AspNetCore.Identity;

namespace Models.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid CurrentCycleId { get; set; }
    public float CycleLength { get; set; }
}
