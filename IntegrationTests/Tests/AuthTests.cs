using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models.Dto.Identity;
using Models.Identity;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class AuthTests
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceScope _scope;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthTests(VitalApiFactory waf)
    {
        _client = waf.Client;
        _scope = waf.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    [Fact]
    public async Task Register_with_insufficient_email()
    {
        var registerRequest = new RegisterRequestDto()
        {
            Email = "userapp",
            Password = "P@ssw0rd.+"
        };
        
        var response = await _client.PostAsync("/Identity/Auth/Register", JsonContent.Create(registerRequest));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == registerRequest.Email)
            .Should().BeNull();
    }
    
    [Fact]
    public async Task Register_with_insufficient_password_length_less_than_6()
    {
        var registerRequest = new RegisterRequestDto()
        {
            Email = "user@app",
            Password = "P@5sw"
        };
        
        var response = await _client.PostAsync("/Identity/Auth/Register", JsonContent.Create(registerRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == registerRequest.Email)
            .Should().BeNull();
    }
    
    [Fact]
    public async Task Register_with_insufficient_password_no_uppercase()
    {
        var registerRequest = new RegisterRequestDto()
        {
            Email = "user@app",
            Password = "p@ssw0rd.+"
        };
        
        var response = await _client.PostAsync("/Identity/Auth/Register", JsonContent.Create(registerRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == registerRequest.Email)
            .Should().BeNull();
    }
    
    [Fact]
    public async Task Register_with_insufficient_password_no_symbol()
    {
        var registerRequest = new RegisterRequestDto()
        {
            Email = "user@app",
            Password = "Passw0rd"
        };
        
        var response = await _client.PostAsync("/Identity/Auth/Register", JsonContent.Create(registerRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == registerRequest.Email)
            .Should().BeNull();
    }
    
    [Fact]
    public async Task Register_with_insufficient_password_no_number()
    {
        var registerRequest = new RegisterRequestDto()
        {
            Email = "user@app",
            Password = "P@ssword.+"
        };
        
        var response = await _client.PostAsync("/Identity/Auth/Register", JsonContent.Create(registerRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == registerRequest.Email)
            .Should().BeNull();
    }
    
    [Fact]
    public async Task Successful_register()
    {
        var registerRequest = new RegisterRequestDto()
        {
            Email = "user@app.com",
            Password = "P@ssw0rd.+"
        };
        
        var response = await _client.PostAsync("/Identity/Auth/Register", JsonContent.Create(registerRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == registerRequest.Email)
            .Should().NotBeNull();
    }
    
    [Fact]
    public async Task Failed_login_user_not_found()
    {
        var loginRequest = new LoginRequestDto()
        {
            Email = "not-found@app.com",
            Password = "P@ssw0rd.+"
        };
        
        var response = await _client.PostAsync("/Identity/Auth/Login", JsonContent.Create(loginRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == loginRequest.Email)
            .Should().BeNull();
    }
    
    [Fact]
    public async Task Failed_login_user_not_confirmed()
    {
        var loginRequest = new LoginRequestDto()
        {
            Email = "not-confirmed@app.com",
            Password = "P@ssw0rd.+"
        };

        var user = new ApplicationUser()
        {
            Email = loginRequest.Email,
            UserName = loginRequest.Email,
            EmailConfirmed = false
        };
        await _userManager.CreateAsync(user, loginRequest.Password);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == loginRequest.Email)
            .Should().NotBeNull();
        
        var response = await _client.PostAsync("/Identity/Auth/Login", JsonContent.Create(loginRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Successful_login_user()
    {
        var loginRequest = new LoginRequestDto()
        {
            Email = "succesful@app.com",
            Password = "P@ssw0rd.+"
        };

        var user = new ApplicationUser()
        {
            Email = loginRequest.Email,
            UserName = loginRequest.Email,
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(user, loginRequest.Password);
        
        _dbContext.Users.FirstOrDefault(u => u.UserName == loginRequest.Email)
            .Should().NotBeNull();
        
        var response = await _client.PostAsync("/Identity/Auth/Login", JsonContent.Create(loginRequest));
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
