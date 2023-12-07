using System.ComponentModel.DataAnnotations;

namespace Models.Dto.Identity.Account;

public class ForgotPasswordDto
{
    [EmailAddress]
    public string Email { get; set; } = null!;
}
