using System.ComponentModel.DataAnnotations;

namespace Models.Dto.InitialLogin;

public class InitialLoginPutDto
{
    [Required]
    [Range(0.0, 100, ErrorMessage = "The field {0} must be greater than {1}.")]
    public required float CycleLength { get; set; }

    [Required]
    [Range(0.0, 100, ErrorMessage = "The field {0} must be greater than {1}.")]
    public required float PeriodLength { get; set; }

    [Required]
    public required DateTimeOffset LastPeriodStart { get; set; }
    public DateTimeOffset? LastPeriodEnd { get; set; }
}
