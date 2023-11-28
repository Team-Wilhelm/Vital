namespace Models.Dto.Metrics;

public class MetricRegisterMetricDto
{
    public Guid MetricsId { get; set; }
    // MetricValue is nullable because a user does not need to set a value for a metric, but can still have a metric, for example a headache can be logged without a severity 
    public Guid? MetricValueId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
