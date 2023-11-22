using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Days;
using Models.Identity;
using Models.Util;

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

        // Add metrics
        await _context.Metrics.AddRangeAsync(
            new Metrics()
            {
                Id = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"),
                Name = "Flow",
                Values = new List<MetricValue>()
            {
                new()
                {
                    Id = Guid.Parse("abe0a53d-bc66-417d-a99a-4490a7bd0640"),
                    Name = "None",
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241")
                },
                new()
                {
                    Id = Guid.Parse("c5cd051e-2990-4171-8e2f-268b0bfc59e0"),
                    Name = "Light",
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241")
                },
                new()
                {
                    Id = Guid.Parse("24d4c8aa-614d-467f-9afb-9e2f744cf151"),
                    Name = "Moderate",
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241")
                },
                new()
                {
                    Id = Guid.Parse("b5bf508b-9cd5-4c9c-aa64-63bc9cbafe3b"),
                    Name = "Heavy",
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241")
                }
            },
            }, new Metrics()
            {
                Id = Guid.Parse("51DA609D-4477-47B8-B1B5-E4298A729D03"),
                Name = "Cramps",
                Values = new List<MetricValue>()
                {
                    new()
                    {
                        Id = Guid.Parse("65C85602-CA96-40F2-8CD1-268DCBFDA131"),
                        Name = "None",
                        MetricsId = Guid.Parse("51DA609D-4477-47B8-B1B5-E4298A729D03")
                    },
                    new()
                    {
                        Id = Guid.Parse("FA7BFF97-3C82-448A-A9E7-C346880D3264"),
                        Name = "Light",
                        MetricsId = Guid.Parse("51DA609D-4477-47B8-B1B5-E4298A729D03")
                    },
                    new()
                    {
                        Id = Guid.Parse("F0F0F0F0-0F0F-0F0F-0F0F-0F0F0F0F0F0F"),
                        Name = "Moderate",
                        MetricsId = Guid.Parse("51DA609D-4477-47B8-B1B5-E4298A729D03")
                    },
                    new()
                    {
                        Id = Guid.Parse("F1F1F1F1-1F1F-1F1F-1F1F-1F1F1F1F1F1F"),
                        Name = "Severe",
                        MetricsId = Guid.Parse("51DA609D-4477-47B8-B1B5-E4298A729D03")
                    }

                }
            }
            );

        // Add cycle days
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow,
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A")
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("0029A2AF-4FC7-497F-BFEC-6E32CDC12623"),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-1),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriodDay = true,
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId = Guid.Parse("0029A2AF-4FC7-497F-BFEC-6E32CDC12623"),
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"),
                    MetricValueId = Guid.Parse("b5bf508b-9cd5-4c9c-aa64-63bc9cbafe3b")
                }
            }
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

        // Link cycle to user's current cycle
        user.CurrentCycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A");
        await _userManager.UpdateAsync(user);

        await _context.SaveChangesAsync();
    }
}
