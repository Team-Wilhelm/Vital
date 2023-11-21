namespace Models.Dto.Cycle;

public class CalendarDayDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public Guid UserId { get; set; }
    public List<MetricsDto> Metrics { get; set; }
    public string State { get; set; }
}
