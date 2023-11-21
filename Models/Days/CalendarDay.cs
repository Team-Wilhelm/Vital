using Models.Identity;
using Models.Util;

namespace Models.Days;

public abstract class CalendarDay
{
    public Guid Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string State { get; set; }   //public string State { get; set; } = "Period"; // Period, Pregnancy, Menopause
    
    public ICollection<CalendarDayMetric> SelectedMetrics { get; set; }
}
