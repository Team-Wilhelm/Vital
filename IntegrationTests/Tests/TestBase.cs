using System.Net.Http.Json;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Dto.Identity;
using Models.Identity;
using Models.Responses;

namespace IntegrationTests.Tests;

public abstract class TestBase
{
    protected readonly HttpClient _client;
    protected readonly ApplicationDbContext _dbContext;
    protected readonly UserManager<ApplicationUser> _userManager;

    protected TestBase(VitalApiFactory vaf)
    {
        _client = vaf.Client;
        var scope = vaf.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    protected async Task AuthorizeUserAndSetHeaderAsync(string email = "user@application",
        string password = "P@ssw0rd.+")
    {
        var loginRequestDto = new LoginRequestDto()
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/identity/auth/login", loginRequestDto);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (authResponse != null)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authResponse.Token}");
        }
    }

    protected Task ClearToken()
    {
        _client.DefaultRequestHeaders.Remove("Authorization");
        return Task.CompletedTask;
    }

    protected async Task<ApplicationUser> RegisterNewUserAndVerifyEmailAsync(string email)
    {
        var user = new ApplicationUser()
        {
            Email = email,
            UserName = email,
            EmailConfirmed = true,
            CycleLength = null,
            PeriodLength = null
        };
        await _userManager.CreateAsync(user, "P@ssw0rd.+");
        await _userManager.AddToRoleAsync(user, "User");
        _dbContext.ChangeTracker.Clear();
        return user;
    }

    protected async Task RemoveUserAsync(string email)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Email == email);
        await _userManager.DeleteAsync(user);
        _dbContext.ChangeTracker.Clear();
    }
}
