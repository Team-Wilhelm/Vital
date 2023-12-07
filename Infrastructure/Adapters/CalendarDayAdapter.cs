namespace Infrastructure.Adapters;

public class CalendarDayAdapter
{
    /// <summary>
    /// CalendarDay
    /// Retrieved as Id
    /// </summary>
    public Guid CalendarDayId { get; set; }

    /// <summary>
    /// Date of the calendar day
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// User Id that the calendar day belongs to
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// State of the calendar day
    /// </summary>
    /// <example>CycleDay,PregnancyDay,MenopauseDay</example>
    public string State { get; set; } = null!;

    /// <summary>
    /// CycleId of the cycle that the calendar day belongs to
    /// </summary>
    public Guid? CycleId { get; set; }

    /// <summary>
    /// Indicates if the calendar day is a period day
    /// </summary>
    public bool IsPeriod { get; set; }

    // Calendar Day Metric
    /// <summary>
    /// Calendar Day Metric
    /// Retrieved as Id
    /// </summary>
    public Guid CalendarDayMetricId { get; set; }

    /// <summary>
    /// Id of the metric that the calendar day metric belongs to
    /// </summary>
    public Guid MetricsId { get; set; }

    /// <summary>
    /// Id of the metric value that the calendar day metric belongs to
    /// </summary>
    public Guid? MetricValueId { get; set; }

    /// <summary>
    /// Date when the calendar day metric was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    // Metrics
    /// <summary>
    /// Name of the metric
    /// </summary>
    public string MetricName { get; set; } = null!;

    // MetricValue
    /// <summary>
    /// Name of the metric value
    /// </summary>
    public string MetricValueName { get; set; } = null!;
}
