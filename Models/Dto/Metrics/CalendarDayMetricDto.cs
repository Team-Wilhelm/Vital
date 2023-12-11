namespace Models.Dto.Metrics;

public class CalendarDayMetricDto
{
    public Guid Id { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public Guid CalendarDayId { get; init; }
    public Guid MetricsId { get; init; }
    public Models.Metrics Metrics { get; init; } = null!;
    public Guid? MetricValueId { get; init; }
}
