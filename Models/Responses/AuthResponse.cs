namespace Models.Responses;

public class AuthResponse
{
    public string Email { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}
