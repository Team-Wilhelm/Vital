using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;
using Models.Dto.Identity.Account;
using Models.Identity;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class AccountTests(VitalApiFactory vaf) : TestBase(vaf)
{
    [Fact]
    public async Task Forgot_Password_input_not_address_return_400()
    {
        var forgotPasswordDto = new ForgotPasswordDto()
        {
            Email = "userapp"
        };

        var response =
            await _client.PostAsync("/Identity/Account/Forgot-Password", JsonContent.Create(forgotPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Forgot_Password_user_not_found_return_200()
    {
        var forgotPasswordDto = new ForgotPasswordDto()
        {
            Email = "forgot-password-not-found@app.com"
        };

        _dbContext.Users.FirstOrDefault(u => u.UserName == forgotPasswordDto.Email).Should().BeNull();

        var response =
            await _client.PostAsync("/Identity/Account/Forgot-Password", JsonContent.Create(forgotPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Forgot_Password_return_500()
    {
        var forgotPasswordDto = new ForgotPasswordDto()
        {
            Email = "forgot-password@app.com"
        };

        var user = new ApplicationUser()
        {
            Email = forgotPasswordDto.Email,
            UserName = forgotPasswordDto.Email,
            EmailConfirmed = true
        };

        await _userManager.CreateAsync(user, "P@ssw0rd.+");

        _dbContext.Users.FirstOrDefault(u => u.UserName == forgotPasswordDto.Email).Should().NotBeNull();

        var response =
            await _client.PostAsync("/Identity/Account/Forgot-Password", JsonContent.Create(forgotPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Reset_Password_input_password_null_return_400()
    {
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = Guid.NewGuid(),
            NewPassword = null!,
            Token = "token",
        };

        var response =
            await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Reset_Password_input_token_null_return_400()
    {
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = Guid.NewGuid(),
            NewPassword = "password",
            Token = null!,
        };

        var response =
            await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Reset_Password_user_not_found_return_400()
    {
        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = Guid.NewGuid(),
            NewPassword = "password",
            Token = "token",
        };

        _dbContext.Users.FirstOrDefault(u => u.Id == resetPasswordDto.UserId).Should().BeNull();

        var response =
            await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

        var response =
            await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

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

        var response =
            await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

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

        var token = await _userManager.GeneratePasswordResetTokenAsync(user!);
        user!.ResetPasswordTokenExpirationDate = DateTime.UtcNow.AddHours(24);
        await _userManager.UpdateAsync(user);

        var resetPasswordDto = new ResetPasswordDto()
        {
            UserId = user.Id,
            NewPassword = "P@ssw0rd.+",
            Token = token
        };


        var response =
            await _client.PostAsync("/Identity/Account/Reset-Password", JsonContent.Create(resetPasswordDto));

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
            UserId = user!.Id,
            Token = "token"
        };

        var response = await _client.PostAsync("/Identity/Account/Verify-Email", JsonContent.Create(verifyRequestDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Verify_email_user_already_verified_return_400()
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

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user!);
        var verifyRequestDto = new VerifyRequestDto()
        {
            UserId = user!.Id,
            Token = token
        };

        var response = await _client.PostAsync("/Identity/Account/Verify-Email", JsonContent.Create(verifyRequestDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user!);
        var verifyRequestDto = new VerifyRequestDto()
        {
            UserId = user!.Id,
            Token = token
        };

        var response = await _client.PostAsync("/Identity/Account/Verify-Email", JsonContent.Create(verifyRequestDto));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Change_Password_Invalid_Model_State_Returns_Bad_Request()
    {
        var changePasswordDto = new ChangePasswordDto()
        {
            OldPassword = "oldPassword",
            NewPassword = "newPassword"
        };

        var response =
            await _client.PostAsync("/Identity/Account/change-password", JsonContent.Create(changePasswordDto));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Change_Password_Valid_Old_Password_Changes_Password()
    {
        await ClearToken();
        // Arrange
        var newUser = new ApplicationUser()
        {
            Email = "change-password@app.com",
            UserName = "change-password@app.com",
            EmailConfirmed = true
        };
        var oldPassword = "P@ssw0rd.+";
        await _userManager.CreateAsync(newUser, oldPassword);
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "change-password@app.com");
        await AuthorizeUserAndSetHeaderAsync(user.Email!);

        var changePasswordDto = new ChangePasswordDto()
        {
            OldPassword = oldPassword,
            NewPassword = "NewP@ssw0rd.+"
        };

        // Act
        var response =
            await _client.PostAsync("/Identity/Account/change-password", JsonContent.Create(changePasswordDto));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Cleanup
        await ClearToken();
    }

    [Fact]
    public async Task Change_Password_Invalid_Old_Password_Returns_BadRequest()
    {
        await ClearToken();
        // Arrange
        var newUser = new ApplicationUser()
        {
            Email = "change-password-invalid@app.com",
            UserName = "change-password-invalid@app.com",
            EmailConfirmed = true
        };
        await _userManager.CreateAsync(newUser, "P@ssw0rd.+");
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "change-password-invalid@app.com");
        await AuthorizeUserAndSetHeaderAsync(user.Email!);

        var changePasswordDto = new ChangePasswordDto()
        {
            OldPassword = "InvalidOldPassword",
            NewPassword = "NewP@ssw0rd.+"
        };

        // Act
        var response =
            await _client.PostAsJsonAsync("/Identity/Account/change-password", changePasswordDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        await ClearToken();
    }
}
