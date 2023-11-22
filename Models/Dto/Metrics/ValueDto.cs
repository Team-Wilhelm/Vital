namespace Models.Dto.Metrics;

public class ValueDto
{
    public Guid Id { get; set; }
    /// <example> "Mild" "Severe"... </example>
    public string Name { get; set; } = null!;
}
