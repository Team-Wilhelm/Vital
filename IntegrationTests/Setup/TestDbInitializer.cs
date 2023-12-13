using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Days;
using Models.Dto.Metrics;
using Models.Identity;
using Vital.Migrations;

namespace IntegrationTests.Setup;

public class TestDbInitializer : DbInitializerHelper
{
   

    public TestDbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager) : base(context, userManager, roleManager)
    {
    }

    public async Task Init()
    {
        await Init(true);
    }
}
