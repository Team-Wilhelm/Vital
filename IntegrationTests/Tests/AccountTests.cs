using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Models.Dto.Identity;
using Models.Dto.Identity.Account;
using Models.Identity;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class AccountTests
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceScope _scope;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountTests(VitalApiFactory waf)
    {
        _client = waf.Client;
        _scope = waf.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    [Fact]
    public async Task Forgot_Password_input_not_address_return_400()
    {
        var forgotPassordDto = new ForgotPasswordDto()
        {
            Email = "userapp"
        };

        var response = await _client.PostAsync("/Identity/Account/Forgot-Password", JsonContent.Create(forgotPassordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Forgot_Password_user_not_found_return_200()
    {
        var forgotPassordDto = new ForgotPasswordDto()
        {
            Email = "forgot-password-not-found@app.com"
        };

        _dbContext.Users.FirstOrDefault(u => u.UserName == forgotPassordDto.Email).Should().BeNull();

        var response = await _client.PostAsync("/Identity/Account/Forgot-Password", JsonContent.Create(forgotPassordDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Forgot_Password_return_500()
    {
        var forgotPassordDto = new ForgotPasswordDto()
        {
            Email = "forgot-password@app.com"
        };
        
        var user = new ApplicationUser()
        {
            Email = forgotPassordDto.Email,
            UserName = forgotPassordDto.Email,
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(user, "P@ssw0rd.+");

        _dbContext.Users.FirstOrDefault(u => u.UserName == forgotPassordDto.Email).Should().NotBeNull();

        var response = await _client.PostAsync("/Identity/Account/Forgot-Password", JsonContent.Create(forgotPassordDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Reset_Password_input_password_null_return_400()
    {
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = Guid.NewGuid(),
            NewPassword = null,
            Token = "token",
        };

        var response = await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Reset_Password_input_token_null_return_400()
    {
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = Guid.NewGuid(),
            NewPassword = "password",
            Token = null,
        };

        var response = await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Reset_Password_user_not_found_return_200()
    {
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = Guid.NewGuid(),
            NewPassword = "password",
            Token = "token",
        };

        _dbContext.Users.FirstOrDefault(u => u.Id == resetPasswordDto.UserId).Should().BeNull();

        var response = await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Reset_Password_invalid_token_return_400()
    {
        var user = new ApplicationUser()
        {
            Email = "reset-password-invalid-token@app.com",
            UserName = "reset-password-invalid-token@app.com",
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(user, "P@ssw0rd.+");
        
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = user.Id,
            NewPassword = "P@ssw0rd.+",
            Token = "token",
        };
        
        _dbContext.Users.FirstOrDefault(u => u.Id == resetPasswordDto.UserId).Should().NotBeNull();

        var response = await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Reset_Password_invalid_password_return_400()
    {
        var user = new ApplicationUser()
        {
            Email = "reset-password-invalid-password@app.com",
            UserName = "reset-password-invalid-password@app.com",
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(user, "P@ssw0rd.+");
        
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = user.Id,
            NewPassword = "invalid-password",
            Token = await _userManager.GeneratePasswordResetTokenAsync(user)
        };
        
        _dbContext.Users.FirstOrDefault(u => u.Id == resetPasswordDto.UserId).Should().NotBeNull();

        var response = await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Reset_Password_return_200()
    {
        var user = new ApplicationUser()
        {
            Email = "reset-password@app.com",
            UserName = "reset-password@app.com",
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(user, "P@ssw0rd.+");

        user = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
        user.Should().NotBeNull();
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = user.Id,
            NewPassword = "P@ssw0rd.+",
            Token = token
        };
        

        var response = await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Verify_email_user_not_found_return_400()
    {
        var verifyRequestDto = new VerifyRequestDto()
        {
            UserId = Guid.NewGuid(),
            Token = "token"
        };

        _dbContext.Users.FirstOrDefault(u => u.Id == verifyRequestDto.UserId).Should().BeNull();
        
        var response = await _client.PostAsync("/Identity/Account/Verify-Email", JsonContent.Create(verifyRequestDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Verify_email_user_invalid_token_return_400()
    {
        var user = new ApplicationUser()
        {
            Email = "verify-email-invalid-token@app.com",
            UserName = "verify-email-invalid-token@app.com",
            EmailConfirmed = false
        };
        await _userManager.CreateAsync(user, "P@ssw0rd.+");

        user = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
        user.Should().NotBeNull();
        
        var verifyRequestDto = new VerifyRequestDto()
        {
            UserId = user.Id,
            Token = "token"
        };

        var response = await _client.PostAsync("/Identity/Account/Verify-Email", JsonContent.Create(verifyRequestDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Verify_email_user_already_verified_return_200()
    {
        var user = new ApplicationUser()
        {
            Email = "verify-email-already-verified@app.com",
            UserName = "verify-email-already-verified@app.com",
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(user, "P@ssw0rd.+");

        user = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
        user.Should().NotBeNull();
        
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var verifyRequestDto = new VerifyRequestDto()
        {
            UserId = user.Id,
            Token = token
        };

        var response = await _client.PostAsync("/Identity/Account/Verify-Email", JsonContent.Create(verifyRequestDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Verify_email_return_200()
    {
        var user = new ApplicationUser()
        {
            Email = "verify-email@app.com",
            UserName = "verify-email@app.com",
            EmailConfirmed = false
        };
        await _userManager.CreateAsync(user, "P@ssw0rd.+");

        user = _dbContext.Users.FirstOrDefault(u => u.Id == user.Id);
        user.Should().NotBeNull();
        
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var verifyRequestDto = new VerifyRequestDto()
        {
            UserId = user.Id,
            Token = token
        };

        var response = await _client.PostAsync("/Identity/Account/Verify-Email", JsonContent.Create(verifyRequestDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
