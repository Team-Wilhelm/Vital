using System.ComponentModel.DataAnnotations;

namespace Models.Dto.Cycle;

public class PeriodAndCycleLengthDto
{
    [Range(0.0, 100, ErrorMessage = "The field {0} must be greater than {1}.")]
    public float CycleLength { get; set; } = 28;

    [Range(0.0, 100, ErrorMessage = "The field {0} must be greater than {1}.")]
    public float PeriodLength { get; set; } = 5;
}
