using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Days;
using Models.Dto.Metrics;
using Models.Identity;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    // Data models

    public DbSet<Cycle> Cycles { get; set; } = null!;
    public DbSet<CycleDay> CycleDays { get; set; } = null!;
    public DbSet<MenopauseDay> MenopauseDays { get; set; } = null!;
    public DbSet<PregnancyDay> PregnancyDays { get; set; } = null!;
    public DbSet<Metrics> Metrics { get; set; } = null!;
    public DbSet<MetricValue> MetricValue { get; set; } = null!;
    public DbSet<CalendarDayMetric> CalendarDayMetric { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalendarDay>()
            .HasDiscriminator(b => b.State);

        base.OnModelCreating(modelBuilder);
    }
}
