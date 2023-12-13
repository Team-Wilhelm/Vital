using System.ComponentModel.DataAnnotations;

namespace Models;

public class Metrics
{
    public Guid Id { get; set; }
    [MaxLength(30)]
    public required string Name { get; set; }
    public List<MetricValue> Values { get; set; } = new();
}
