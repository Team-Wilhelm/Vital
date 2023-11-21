namespace Models.Dto.Cycle;

public class CycleDayDto : CalendarDayDto
{
    public Guid CycleId { get; set; }
    public bool IsPeriod { get; set; } //Maybe remove this
}
