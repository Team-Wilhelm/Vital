using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Days;
using Models.Identity;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    // Data models

    public DbSet<Cycle> Cycles { get; set; }
    public DbSet<CycleDay> CycleDays { get; set; }
    public DbSet<MenopauseDay> MenopauseDays { get; set; }
    public DbSet<PregnancyDay> PregnancyDays { get; set; }
    
    public DbSet<Metrics> Metrics { get; set; }
    

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
