using Models.Identity;

namespace Models.Dto.Identity.Account;

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
