using System.ComponentModel.DataAnnotations;
using Models.Dto.Metrics;
using Models.Identity;

namespace Models.Days;

public abstract class CalendarDay
{
    public Guid Id { get; init; }
    public DateTimeOffset Date { get; init; }
    public Guid UserId { get; init; }
    public ApplicationUser? User { get; init; }
    [MaxLength(20)]
    public string? State { get; init; }   //public string State { get; set; } = "CycleDay"; // CycleDay, PregnancyDay, MenopauseDay

    public ICollection<CalendarDayMetric> SelectedMetrics { get; set; } = new List<CalendarDayMetric>();
}
