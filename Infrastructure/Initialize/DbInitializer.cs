using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Days;
using Models.Dto.Metrics;
using Models.Identity;

namespace Infrastructure.Initialize;

public class DbInitializer : DbInitializerHelper
{
    public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : base(context, userManager, roleManager)
    {
    }
    
    public async Task Init()
    {
        await Init(false);
    }
}
