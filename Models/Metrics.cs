namespace Models;

public class Metrics
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<MetricValue> Values { get; set; } = new();
}
