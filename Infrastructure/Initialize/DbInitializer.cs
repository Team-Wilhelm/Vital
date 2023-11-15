using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Days;
using Models.Identity;

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
            Id = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            StartDate = DateTimeOffset.UtcNow.AddDays(-10),
            EndDate = DateTimeOffset.UtcNow.AddDays(10)
        });

        // Add cycle days
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow,
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriodDay = true
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-1),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriodDay = true
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-2),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriodDay = true
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-3),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriodDay = true

        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-4),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriodDay = true
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-5),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriodDay = false
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-6),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriodDay = false
        });

        await _context.SaveChangesAsync();
    }
}
