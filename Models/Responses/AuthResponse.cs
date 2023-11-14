namespace Models.Responses; 

public class AuthResponse {
    public string Email { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public List<string> Roles { get; set; }
}
