using System.ComponentModel.DataAnnotations;

namespace Models;

public class MetricValue
{
    public Guid Id { get; set; }
    public Guid MetricsId { get; set; }
    /// <example> "Mild" "Severe"... </example>
    [MaxLength(30)]
    public required string Name { get; set; }
}
