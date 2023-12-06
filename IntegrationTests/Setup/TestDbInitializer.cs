using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Days;
using Models.Identity;
using Models.Util;

namespace IntegrationTests.Setup;

public class TestDbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public TestDbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task Init()
    {
         if (_roleManager.Roles.SingleOrDefault(r => r.Name == "User") == null)
        {
            await _roleManager.CreateAsync(new ApplicationRole
            {
                Name = "User"
            });
        }

        // User 1 with a finished period
        var user1 = new ApplicationUser()
        {
            Id = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            UserName = "user@application",
            Email = "user@application",
            EmailConfirmed = true,
        };
        await _userManager.CreateAsync(user1, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user1, "User");

        await _context.Cycles.AddAsync(new Cycle()
        {
            Id = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            StartDate = DateTimeOffset.UtcNow.AddDays(-10),
        });
        
        // User 2 with a current period
        var user2 = new ApplicationUser()
        {
            Id = Guid.Parse("B1F0B1F0-B1F0-B1F0-B1F0-B1F0B1F0B1F0"),
            UserName = "user2@application",
            Email = "user2@application",
            EmailConfirmed = true,
        };
        
        await _userManager.CreateAsync(user2, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user2, "User");
        
        await _context.Cycles.AddAsync(new Cycle()
        {
            Id = Guid.Parse("EA2DCAC0-47C5-4406-BA1C-FA870EE5577E"),
            UserId = Guid.Parse("B1F0B1F0-B1F0-B1F0-B1F0-B1F0B1F0B1F0"),
            StartDate = DateTimeOffset.UtcNow.AddDays(-2),
        });
        
        // User 3 with no logged metrics
        var user3 = new ApplicationUser()
        {
            Id = Guid.Parse("C1F0C1F0-B1F0-B1F0-B1F0-B1F0B1F0B1F0"),
            UserName = "user3@application",
            Email = "user3@application",
            EmailConfirmed = true,
        };
        
        await _userManager.CreateAsync(user3, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user3, "User");
        
        await _context.Cycles.AddAsync(new Cycle()
        {
            Id = Guid.Parse("A2E8D29E-6734-4EF5-9155-93EF6C995EF8"),
            UserId = Guid.Parse("C1F0C1F0-B1F0-B1F0-B1F0-B1F0B1F0B1F0"),
            StartDate = DateTimeOffset.UtcNow.AddDays(-2)
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
                },
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

        // Add cycle days for user 1
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("0029A2AF-4FC7-497F-BFEC-6E32CDC12623"),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-5),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriod = true,
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId = Guid.Parse("0029A2AF-4FC7-497F-BFEC-6E32CDC12623"),
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"), // Flow
                    MetricValueId = Guid.Parse("b5bf508b-9cd5-4c9c-aa64-63bc9cbafe3b"), // Heavy
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-5).AddHours(-5)
                }
            }
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("E429A2AF-4FC7-497F-BFEC-6E32CDC12623"),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-4),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriod = true,
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId = Guid.Parse("E429A2AF-4FC7-497F-BFEC-6E32CDC12623"),
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"), // Flow
                    MetricValueId = Guid.Parse("b5bf508b-9cd5-4c9c-aa64-63bc9cbafe3b"), // Heavy
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-4).AddHours(-4)
                }
            }
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("9B294EA6-0440-427F-84D1-8058AEDB3B12"),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-3),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriod = true,
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId = Guid.Parse("9B294EA6-0440-427F-84D1-8058AEDB3B12"),
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"), // Flow
                    MetricValueId = Guid.Parse("b5bf508b-9cd5-4c9c-aa64-63bc9cbafe3b"), // Heavy
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-3).AddHours(-3)
                }
            }

        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("388725C0-63AD-4EC8-A5E5-E760ACFCB0F0"),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-2),
            IsPeriod = true,
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId = Guid.Parse("388725C0-63AD-4EC8-A5E5-E760ACFCB0F0"),
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"), // Flow
                    MetricValueId = Guid.Parse("24d4c8aa-614d-467f-9afb-9e2f744cf151"), // Moderate
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-2).AddHours(-2)
                }
            }
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
            Date = DateTimeOffset.UtcNow.AddDays(-1),
            CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            IsPeriod = true,
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId = Guid.Parse("388725C0-63AD-4EC8-A5E5-E760ACFCB0F0"),
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"), // Flow
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1).AddHours(-1)
                }
            }
        });
        await _context.CycleDays.AddAsync(new CycleDay()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("ADFEAD4C-823B-41E5-9C7E-C84AA04192A4"),
                Date = DateTimeOffset.UtcNow,
                IsPeriod = false,
                CycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A")
        });
       
        // Link cycle to user's current cycle
        user1.CurrentCycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A");
        await _userManager.UpdateAsync(user1);
        
        // Add cycle days for user 2
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("EFE6886A-374D-48E2-A3E7-16637865ED74"),
            UserId = Guid.Parse("B1F0B1F0-B1F0-B1F0-B1F0-B1F0B1F0B1F0"),
            Date = DateTimeOffset.UtcNow.AddDays(-1),
            CycleId = Guid.Parse("EA2DCAC0-47C5-4406-BA1C-FA870EE5577E"),
            IsPeriod = true,
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId =  Guid.Parse("EFE6886A-374D-48E2-A3E7-16637865ED74"),
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"), // Flow
                    MetricValueId = Guid.Parse("24d4c8aa-614d-467f-9afb-9e2f744cf151"), // Moderate
                    CreatedAt = DateTimeOffset.UtcNow.AddDays(-1).AddHours(-1)
                }
            }
        });
        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("F0121084-6054-4278-AA9A-246A7AEFD11A"),
            UserId = Guid.Parse("b1f0b1f0-b1f0-b1f0-b1f0-b1f0b1f0b1f0"),
            Date = DateTimeOffset.UtcNow,
            IsPeriod = true,
            CycleId = Guid.Parse("EA2DCAC0-47C5-4406-BA1C-FA870EE5577E"),
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId =  Guid.Parse("F0121084-6054-4278-AA9A-246A7AEFD11A"),
                    MetricsId = Guid.Parse("d56807fe-05ca-4901-a564-68f14e31b241"), // Flow
                    MetricValueId = Guid.Parse("24d4c8aa-614d-467f-9afb-9e2f744cf151"), // Moderate
                    CreatedAt = DateTimeOffset.UtcNow
                }
            }
        });
        
        // Link cycle to user's current cycle
        user2.CurrentCycleId = Guid.Parse("EA2DCAC0-47C5-4406-BA1C-FA870EE5577E");
        await _userManager.UpdateAsync(user2);
        
        await _context.SaveChangesAsync();
    }
}
