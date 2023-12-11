using System.ComponentModel.DataAnnotations;
using Models.Identity;

namespace Models.Dto.Identity.Account;

public class ChangePasswordDto
{
    [Required]
    [StringLength(100, MinimumLength = 6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$")]
    public string OldPassword { get; set; } = null!;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$")]
    public string NewPassword { get; set; } = null!;
}
