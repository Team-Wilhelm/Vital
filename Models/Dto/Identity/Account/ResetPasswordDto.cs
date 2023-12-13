using System.ComponentModel.DataAnnotations;

namespace Models.Dto.Identity.Account;

public class ResetPasswordDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$")]
    public string NewPassword { get; set; } = null!;
}
