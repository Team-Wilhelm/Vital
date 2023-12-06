using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Dto.Identity;
using Models.Dto.InitialLogin;
using Models.Identity;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class FirstLoginTests
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceScope _scope;
    private readonly UserManager<ApplicationUser> _userManager;

    public FirstLoginTests(VitalApiFactory waf, ITestOutputHelper testOutputHelper)
    {
        _client = waf.Client;
        _scope = waf.Services.CreateScope();
        _dbContext =  _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    [Fact]
    public async Task Firs_Login_Should_Have_Null_Data()
    {
        await RegisterNewUserAndVerifyEmailAsync("user3@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, "user3@application");

        var response = await _client.GetAsync("/cycle/initial-login");
        var actual = await response.Content.ReadFromJsonAsync<InitialLoginGetDto>();
        var result = actual?.CycleLength == null || actual.PeriodLength == null;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeTrue();

        await Utilities.ClearToken(_client);
        await RemoveUserAsync("user3@application");
    }

    [Fact]
    public async Task First_Login_Should_Be_False()
    {
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client); // user@application
        var response = await _client.GetAsync("/cycle/initial-login");
        var actual = await response.Content.ReadFromJsonAsync<InitialLoginGetDto>();
        var result = actual?.CycleLength == null || actual.PeriodLength == null;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeFalse();
        
        await Utilities.ClearToken(_client);
    }
    
    [Fact]
    public async Task Post_OK_OnGoing_Period()
    {
        const string username = "user4@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, username);

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
        
        await Utilities.ClearToken(_client);
        await RemoveUserAsync(username);
    }
    
    [Fact]
    public async Task Post_OK_Ended_Period()
    {
        const string username = "user5@application";
        await RegisterNewUserAndVerifyEmailAsync(username);
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, username);

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
        
        await Utilities.ClearToken(_client);
        await RemoveUserAsync(username);
    }

    private async Task RegisterNewUserAndVerifyEmailAsync(string email)
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
    }
    
    private async Task RemoveUserAsync(string email)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Email == email);
        await _userManager.DeleteAsync(user);
        _dbContext.ChangeTracker.Clear();
    }
    
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
}
