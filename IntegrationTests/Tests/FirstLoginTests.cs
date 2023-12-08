using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Dto.InitialLogin;
using Models.Identity;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class FirstLoginTests(VitalApiFactory vaf) : TestBase(vaf)
{

    [Fact]
    public async Task Should_Return_Unauthorized_When_Not_Authorized()
    {
        var response = await _client.GetAsync("/cycle/initial-login");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Firs_Login_Should_Have_Null_Data()
    {
        await RegisterNewUserAndVerifyEmailAsync("temp@application");
        await AuthorizeUserAndSetHeaderAsync(_client, "temp@application");

        var response = await _client.GetAsync("/cycle/initial-login");
        var actual = await response.Content.ReadFromJsonAsync<InitialLoginGetDto>();
        var result = actual?.CycleLength == null || actual.PeriodLength == null;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeTrue();

        await ClearToken(_client);
        await RemoveUserAsync("temp@application");
    }

    [Fact]
    public async Task First_Login_Should_Be_False()
    {
        await AuthorizeUserAndSetHeaderAsync(_client); // user@application
        var response = await _client.GetAsync("/cycle/initial-login");
        var actual = await response.Content.ReadFromJsonAsync<InitialLoginGetDto>();
        var result = actual?.CycleLength == null || actual.PeriodLength == null;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeFalse();

        await ClearToken(_client);
    }

    [Fact]
    public async Task Post_OK_OnGoing_Period()
    {
        const string username = "temp@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await AuthorizeUserAndSetHeaderAsync(_client, username);

        var initialLoginPutDto = new InitialLoginPutDto()
        {
            CycleLength = 30,
            PeriodLength = 6,
            LastPeriodStart = DateTime.UtcNow.AddDays(-5)
        };

        var response = await _client.PutAsJsonAsync("/cycle/initial-login", initialLoginPutDto);

        var user = await _dbContext.Users.FirstAsync(u => u.Email == username);
        user.CycleLength.Should().Be(initialLoginPutDto.CycleLength);
        user.PeriodLength.Should().Be(initialLoginPutDto.PeriodLength);
        await AssertUserHasCycleAsync(user, initialLoginPutDto);
        await AssertMetricsWereCreatedAsync(user, initialLoginPutDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await ClearToken(_client);
        await RemoveUserAsync(username);
    }

    [Fact]
    public async Task Post_OK_Ended_Period()
    {
        const string username = "temp@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await AuthorizeUserAndSetHeaderAsync(_client, username);

        var initialLoginPutDto = new InitialLoginPutDto()
        {
            CycleLength = 29,
            PeriodLength = 5,
            LastPeriodStart = DateTime.UtcNow.AddDays(-6),
            LastPeriodEnd = DateTime.UtcNow.AddDays(-1)
        };

        var response = await _client.PutAsJsonAsync("/cycle/initial-login", initialLoginPutDto);

        var user = await _dbContext.Users.FirstAsync(u => u.Email == username);
        user.CycleLength.Should().Be(initialLoginPutDto.CycleLength);
        user.PeriodLength.Should().Be(initialLoginPutDto.PeriodLength);
        await AssertUserHasCycleAsync(user, initialLoginPutDto);
        await AssertMetricsWereCreatedAsync(user, initialLoginPutDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await ClearToken(_client);
        await RemoveUserAsync(username);
    }

    [Fact]
    public async Task Post_BadRequest_Invalid_CycleLength()
    {
        const string username = "temp@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await AuthorizeUserAndSetHeaderAsync(_client, username);

        var initialLoginPutDto = new InitialLoginPutDto()
        {
            CycleLength = -1,
            PeriodLength = 5,
            LastPeriodStart = DateTime.UtcNow.AddDays(-6),
            LastPeriodEnd = DateTime.UtcNow.AddDays(-1)
        };

        var response = await _client.PutAsJsonAsync("/cycle/initial-login", initialLoginPutDto);

        var user = await _dbContext.Users.FirstAsync(u => u.Email == username);
        user.CycleLength.Should().BeNull();
        user.PeriodLength.Should().BeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await ClearToken(_client);
        await RemoveUserAsync(username);
    }

    [Fact]
    public async Task Post_BadRequest_Invalid_PeriodLength()
    {
        const string username = "temp@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await AuthorizeUserAndSetHeaderAsync(_client, username);

        var initialLoginPutDto = new InitialLoginPutDto()
        {
            CycleLength = 29,
            PeriodLength = -1,
            LastPeriodStart = DateTime.UtcNow.AddDays(-6),
            LastPeriodEnd = DateTime.UtcNow.AddDays(-1)
        };

        var response = await _client.PutAsJsonAsync("/cycle/initial-login", initialLoginPutDto);

        var user = await _dbContext.Users.FirstAsync(u => u.Email == username);
        user.CycleLength.Should().BeNull();
        user.PeriodLength.Should().BeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await ClearToken(_client);
        await RemoveUserAsync(username);
    }

    [Fact]
    public async Task Post_BadRequest_Invalid_LastPeriodStart()
    {
        const string username = "temp@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await AuthorizeUserAndSetHeaderAsync(_client, username);

        var initialLoginPutDto = new InitialLoginPutDto()
        {
            CycleLength = 29,
            PeriodLength = 5,
            LastPeriodStart = DateTime.UtcNow.AddDays(1)
        };

        var response = await _client.PutAsJsonAsync("/cycle/initial-login", initialLoginPutDto);
        var responseContent = await response.Content.ReadAsStringAsync();

        var user = await _dbContext.Users.FirstAsync(u => u.Email == username);
        user.CycleLength.Should().BeNull();
        user.PeriodLength.Should().BeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().Be("Last period start and end dates cannot be in the future.");

        await ClearToken(_client);
        await RemoveUserAsync(username);
    }

    [Fact]
    public async Task Post_BadRequest_Invalid_LastPeriodEnd()
    {
        const string username = "temp@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await AuthorizeUserAndSetHeaderAsync(_client, username);

        var initialLoginPutDto = new InitialLoginPutDto()
        {
            CycleLength = 29,
            PeriodLength = 5,
            LastPeriodStart = DateTime.UtcNow.AddDays(-6),
            LastPeriodEnd = DateTime.UtcNow.AddDays(1)
        };

        var response = await _client.PutAsJsonAsync("/cycle/initial-login", initialLoginPutDto);
        var responseContent = await response.Content.ReadAsStringAsync();

        var user = await _dbContext.Users.FirstAsync(u => u.Email == username);
        user.CycleLength.Should().BeNull();
        user.PeriodLength.Should().BeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().Be("Last period start and end dates cannot be in the future.");

        await ClearToken(_client);
        await RemoveUserAsync(username);
    }

    [Fact]
    public async Task Post_BadRequest_Invalid_Period_Start_After_Period_End()
    {
        const string username = "temp@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await AuthorizeUserAndSetHeaderAsync(_client, username);

        var initialLoginPutDto = new InitialLoginPutDto()
        {
            CycleLength = 29,
            PeriodLength = 5,
            LastPeriodStart = DateTime.UtcNow.AddDays(-3),
            LastPeriodEnd = DateTime.UtcNow.AddDays(-6)
        };

        var response = await _client.PutAsJsonAsync("/cycle/initial-login", initialLoginPutDto);
        var responseContent = await response.Content.ReadAsStringAsync();

        var user = await _dbContext.Users.FirstAsync(u => u.Email == username);
        user.CycleLength.Should().BeNull();
        user.PeriodLength.Should().BeNull();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        responseContent.Should().Be("Last period start date must be before the end date.");

        await ClearToken(_client);
        await RemoveUserAsync(username);
    }

    #region Utility methods
    private async Task AssertUserHasCycleAsync(ApplicationUser user, InitialLoginPutDto initialLoginPutDto)
    {
        var cycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id);

        // Assert if cycle was successfully created
        cycle.Should().NotBeNull();
        cycle.StartDate.Date.Should().Be(initialLoginPutDto.LastPeriodStart.Date);
        cycle.EndDate.Should().BeNull();

        // Assert if user is connected to cycle and has correct cycle length
        user.CurrentCycleId.Should().Be(cycle.Id);
    }

    private async Task AssertMetricsWereCreatedAsync(ApplicationUser user, InitialLoginPutDto initialLoginPutDto)
    {
        // Check if metrics were created for the days between last period start and today
        var upperBound = initialLoginPutDto.LastPeriodEnd ?? DateTime.UtcNow;
        var daysBetween = (upperBound - initialLoginPutDto.LastPeriodStart).Days + 1; // + today
        var metrics = await _dbContext.CalendarDayMetric
            .Include(cdm => cdm.Metrics)
            .Include(cdm => cdm.MetricValue)
            .Where(cdm => cdm.CalendarDay!.UserId == user.Id)
            .ToListAsync();
        metrics.Count.Should().Be(daysBetween);
        metrics.ForEach(m =>
        {
            m.Metrics!.Name.Should().Be("Flow");
        });
    }
    #endregion
}
