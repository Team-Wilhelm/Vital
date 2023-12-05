namespace Models.Dto.Identity.Account;

public class VerifyRequestDto
{
    public Guid UserId { get; set; } = default!;
    public string Token { get; set; } = default!;
}
