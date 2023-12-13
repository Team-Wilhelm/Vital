using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Days;
using Models.Dto.Metrics;
using Models.Identity;

namespace Infrastructure;

public abstract class DbInitializerHelper
{
    protected readonly ApplicationDbContext _context;
    protected readonly UserManager<ApplicationUser> _userManager;
    protected readonly RoleManager<ApplicationRole> _roleManager;

    protected DbInitializerHelper(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    protected void AddFlowCycleDays(int amount, Guid cycleId, Guid userId)
    {
        var flowMetric = _context.Metrics
            .Include(metrics => metrics.Values)
            .First(m => m.Name == "Flow");
        var todayAtTwelve = new DateTimeOffset(DateTimeOffset.UtcNow.Date, TimeSpan.Zero).AddHours(12);
        
        
        for (var i = 0; i < amount; i++)
        {
            var calendarDayId = Guid.NewGuid();
            var random = new Random();
            _context.CycleDays.Add(new CycleDay()
            {
                Id = calendarDayId,
                UserId = userId,
                Date = new DateTimeOffset(DateTimeOffset.UtcNow.AddDays(-amount + i).Date, TimeSpan.Zero).AddHours(12),
                CycleId = cycleId,
                IsPeriod = true,
                SelectedMetrics = new List<CalendarDayMetric>()
                {
                    new()
                    {
                        CalendarDayId = calendarDayId,
                        MetricsId = flowMetric.Id, // Flow
                        MetricValueId = flowMetric.Values.Count > 0 
                            ? flowMetric.Values.ToArray()[random.Next(0, flowMetric.Values.Count - 1)].Id
                            : null, // Pick a random value
                        CreatedAt =  new DateTimeOffset(DateTimeOffset.UtcNow.Date.AddDays(-amount + i), TimeSpan.Zero).AddHours(12)
                    }
                }
            });
        }
    }
    
    protected Metrics CreateMetric(string metricName, List<string>? values = null)
    {
        var metric = new Metrics()
        {
            Id = Guid.NewGuid(),
            Name = metricName,
            Values = new List<MetricValue>()
        };

        if (values != null)
        {
            foreach (var value in values)
            {
                metric.Values.Add(new MetricValue()
                {
                    Id = Guid.NewGuid(),
                    Name = value,
                    MetricsId = metric.Id
                });
            }
        }
        return metric;
    }

    protected async Task Init(bool isTesting = false)
    {
        var user1Email = isTesting ? "user@application" : "user@app";
        var user2Email = isTesting ? "user2@application" : "user2@app";
        if (!isTesting)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        
        #region Add metrics
        
        var flowMetric = CreateMetric("Flow", new List<string>() { "Light", "Moderate", "Heavy" });
        
        // Add metrics
        await _context.Metrics.AddRangeAsync(
            flowMetric,
            CreateMetric("Cramps", new List<string>() { "Light", "Moderate", "Severe" }),
            CreateMetric("Headache", new List<string>() { "Light", "Moderate", "Severe" }),
            CreateMetric("Spotting"),
            CreateMetric("Bloating"),
            CreateMetric("Acne"),
            CreateMetric("Fatigue"));
        #endregion

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
            UserName = user1Email,
            Email = user1Email,
            EmailConfirmed = true,
        };
        await _userManager.CreateAsync(user1, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user1, "User");

        var utcNow = DateTimeOffset.UtcNow;
        await _context.Cycles.AddAsync(new Cycle()
        {
            Id = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"),
            UserId = user1.Id,
            StartDate = new DateTimeOffset(utcNow.AddDays(-5).Date, TimeSpan.Zero).AddHours(12)
        });
        
        // Add cycle days for user 1
        AddFlowCycleDays(5, Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A"), user1.Id);

        // Link cycle to user's current cycle
        user1.CurrentCycleId = Guid.Parse("2AF6BC6C-B3C0-4E77-97D9-9FA6D36C4A0A");
        await _userManager.UpdateAsync(user1);

        #region Add more cycles for user 1

        // Cycle 1 - newest
        var cycle = new Cycle()
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            StartDate = new DateTimeOffset(utcNow.AddDays(-6 - (30 * 1)).Date, TimeSpan.Zero).AddHours(12),
            EndDate = new DateTimeOffset(utcNow.AddDays(-5).Date, TimeSpan.Zero).AddHours(12)
        };
        await _context.Cycles.AddAsync(cycle);

        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("C8DCD3C7-8889-4BF0-BE6C-3017F45ACF1A"),
            UserId = user1.Id,
            Date = new DateTimeOffset(utcNow.AddDays(-6 - (30 * 1)).Date, TimeSpan.Zero).AddHours(12),
            CycleId = cycle.Id,
            IsPeriod = true,
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId = Guid.Parse("C8DCD3C7-8889-4BF0-BE6C-3017F45ACF1A"),
                    MetricsId = flowMetric.Id, // Flow
                    MetricValueId = flowMetric.Values.ToArray()[2].Id, // Heavy
                    CreatedAt = new DateTimeOffset(DateTimeOffset.UtcNow.Date.AddDays(-36), TimeSpan.Zero).AddHours(12)
                }
            }
        });
        
        // Cycle 2
        var cycle2 = new Cycle()
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            StartDate = new DateTimeOffset(utcNow.AddDays(-5 - (30 * 2)).Date, TimeSpan.Zero).AddHours(12),
            EndDate = new DateTimeOffset(utcNow.AddDays(-7 - (30 * 1)).Date, TimeSpan.Zero).AddHours(12)
        };
        await _context.Cycles.AddAsync(cycle2);

        await _context.CycleDays.AddAsync(new CycleDay()
        {
            Id = Guid.Parse("14177B9E-B74B-4477-9496-89E10570411D"),
            UserId = user1.Id,
            Date = new DateTimeOffset(utcNow.AddDays(-6 - (30 * 2)).Date, TimeSpan.Zero).AddHours(12),
            CycleId = cycle2.Id,
            IsPeriod = true,
            SelectedMetrics = new List<CalendarDayMetric>()
            {
                new()
                {
                    CalendarDayId = Guid.Parse("14177B9E-B74B-4477-9496-89E10570411D"),
                    MetricsId = flowMetric.Id, // Flow
                    MetricValueId = flowMetric.Values.ToArray()[1].Id, // Moderate
                    CreatedAt = new DateTimeOffset(DateTimeOffset.UtcNow.Date.AddDays(-72), TimeSpan.Zero).AddHours(12)
                }
            }
        });

        // Cycle 3
        var cycle3 = new Cycle()
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            StartDate = new DateTimeOffset(utcNow.AddDays(-5 - (30 * 3)).Date, TimeSpan.Zero).AddHours(12),
            EndDate = new DateTimeOffset(utcNow.AddDays(-6 - (30 * 2)).Date, TimeSpan.Zero).AddHours(12),
        };
        await _context.Cycles.AddAsync(cycle3);


        #endregion

        // User 2 with a current period
        var user2 = new ApplicationUser()
        {
            Id = Guid.Parse("B1F0B1F0-B1F0-B1F0-B1F0-B1F0B1F0B1F0"),
            UserName = user2Email,
            Email = user2Email,
            EmailConfirmed = true,
        };

        await _userManager.CreateAsync(user2, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user2, "User");

        await _context.Cycles.AddAsync(new Cycle()
        {
            Id = Guid.Parse("EA2DCAC0-47C5-4406-BA1C-FA870EE5577E"),
            UserId = user2.Id,
            StartDate = new DateTimeOffset(utcNow.AddDays(-2).Date, TimeSpan.Zero).AddHours(12)
        });
        AddFlowCycleDays(2, Guid.Parse("EA2DCAC0-47C5-4406-BA1C-FA870EE5577E"), user2.Id);
        
        // Link cycle to user's current cycle
        user2.CurrentCycleId = Guid.Parse("EA2DCAC0-47C5-4406-BA1C-FA870EE5577E");
        await _userManager.UpdateAsync(user2);

        await _context.SaveChangesAsync();
    }
}
