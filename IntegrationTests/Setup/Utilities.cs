using System.Net.Http.Json;
using Models.Dto.Identity;
using Models.Responses;

namespace IntegrationTests.Setup;

public static class Utilities
{
    public static async Task AuthorizeUserAndSetHeaderAsync(HttpClient client, string email = "user@app", string password = "P@ssw0rd.+")
    {
        var loginRequestDto = new LoginRequestDto()
        {
            Email = email,
            Password = password
        };
        
        var response = await client.PostAsJsonAsync("/identity/auth/login", loginRequestDto);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (authResponse != null)
        {
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authResponse.Token}");
        }
    }
}
