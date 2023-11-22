namespace Models.Dto.Metrics;

public class CalendarDayMetricDto
{
    public Guid Id { get; set; }
    public Guid CalendarDayId { get; set; }
    public Guid MetricsId { get; set; }
    public Models.Metrics Metrics { get; set; }
    public Guid MetricValueId { get; set; }
}
