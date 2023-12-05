using Models.Days;

namespace Models.Util;

public class CalendarDayMetric
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CalendarDayId { get; set; }
    public CalendarDay? CalendarDay { get; set; }
    public Guid MetricsId { get; set; }
    public Metrics? Metrics { get; set; }
    public Guid? MetricValueId { get; set; } // MetricValue is nullable because a user does not need to set a value for a metric, but can still have a metric
    public MetricValue? MetricValue { get; set; }
}
