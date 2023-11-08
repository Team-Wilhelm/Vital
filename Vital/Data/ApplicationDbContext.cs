using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vital.Models.Identity;

namespace Vital.Data; 

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid> {
    // Data models
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
}
