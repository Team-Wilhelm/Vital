namespace Models;

public class MetricValue
{
    public Guid Id { get; set; }
    public Guid MetricsId { get; set; }
    /// <example> "Mild" "Severe"... </example>
    public required string Name { get; set; }
}
