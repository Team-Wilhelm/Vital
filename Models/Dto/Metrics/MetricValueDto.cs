namespace Models.Dto.Metrics;

public class MetricValueDto
{
    public Guid Id { get; set; }
    public Guid MetricsId { get; set; }
    /// <example> "Mild" "Severe"... </example>
    public required string Name { get; set; }
    public List<Guid> CalendarDayIds { get; set; } = new();
}
