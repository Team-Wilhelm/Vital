namespace Infrastructure.Adapters;

public class CalendarDayAdapter
{
    // CalendarDay
    public Guid CalendarDayId { get; set; } // Retrieved as Id
    public DateTimeOffset Date { get; set; }
    public Guid UserId { get; set; }
    public string State { get; set; }
    public Guid? CycleId { get; set; }
    public bool IsPeriodDay { get; set; }
    
    // CalendarDayMetric
    public Guid CalendarDayMetricId { get; set; } // Retrieved as Id
    public Guid MetricsId { get; set; }
    public Guid MetricValueId { get; set; }
    
    // Metrics
    public string MetricName { get; set; } // Retrieved as Name
    
    // MetricValue
    public string MetricValueName { get; set; } // Retrieved as Name
}
