using Models.Days;

namespace Models.Util;

public class CalendarDayMetric
{
    public Guid Id { get; set; }
    public Guid CalendarDayId { get; set; }
    public CalendarDay CalendarDay { get; set; }
    public Guid MetricsId { get; set; }
    public Metrics Metrics { get; set; }
    public Guid MetricValueId { get; set; }
    public MetricValue MetricValue { get; set; }
}
