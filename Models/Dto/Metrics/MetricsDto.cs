﻿namespace Models.Dto.Metrics;

public class MetricsDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<ValueDto> Values { get; set; } = new();
}
