namespace Models.Dto.Cycle;

public class CycleAnalyticsDto
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public List<DateTimeOffset> PeriodDays { get; set; } = new();

}
