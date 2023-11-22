namespace Models.Dto.Identity.Account;

public class ResetPasswordDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
