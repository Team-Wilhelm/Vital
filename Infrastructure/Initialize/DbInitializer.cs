using Microsoft.AspNetCore.Identity;
using Models;
using Models.Identity;
using Vital.Data;

namespace Infrastructure.Initialize;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Init()
    {
        // Delete and create database
        // to ensure that database is empty
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        if (_roleManager.Roles.SingleOrDefault(r => r.Name == "User") == null)
        {
            await _roleManager.CreateAsync(new ApplicationRole
            {
                Name = "User"
            });
        }

        var user = new ApplicationUser()
        {
            Id = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            UserName = "user@app",
            Email = "user@app",
            EmailConfirmed = true,
        };
        await _userManager.CreateAsync(user, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user, "User");

        await _context.Cycles.AddAsync(new Cycle()
        {

        });


        await _context.SaveChangesAsync();
    }
}
