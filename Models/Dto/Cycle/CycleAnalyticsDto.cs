using Models.Days;

namespace Models.Dto.Cycle;

public class CycleAnalyticsDto
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public List<CycleDay> CycleDays { get; set; } = new();
    
}
