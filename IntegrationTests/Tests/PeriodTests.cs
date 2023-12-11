using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Setup;
using Models.Dto.Cycle;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class PeriodTests(VitalApiFactory vaf) : TestBase(vaf)
{
    [Fact]
    public async Task Get_Predicted_Period_Should_be_unauthorized()
    {
        await ClearToken();

        var response = await _client.GetAsync($"/cycle/predicted-period");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_Predicted_Period_OnGoing_Period_Success()
    {
        var user = _userManager.Users.First(u => u.Email == "user2@application");
        await AuthorizeUserAndSetHeaderAsync( user.Email);

        // The user has the average period length of 5 days and logged 2, so the predicted period should be for the next 3 days
        var response = await _client.GetAsync($"/cycle/predicted-period");
        var actual = await response.Content.ReadFromJsonAsync<List<DateTimeOffset>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        actual.Should().HaveCount(18);

        // The predicted period should be for the next 3 days
        actual[0].Date.Should().Be(DateTimeOffset.Now.Date.AddDays(1));
        actual[1].Date.Should().Be(DateTimeOffset.Now.Date.AddDays(2));
        actual[2].Date.Should().Be(DateTimeOffset.Now.Date.AddDays(3));

        var startDate = _dbContext.Cycles.First(c => c.UserId == user.Id).StartDate.Date;
        AssertFutureCyclePredictions(3, (int)Math.Floor(user.PeriodLength!.Value), startDate, actual);
        
        await ClearToken();
    }

    [Fact]
    public async Task Get_Predicted_Period_Future_Success()
    {
        var user = _userManager.Users.First(u => u.Email == "user@application");
        await AuthorizeUserAndSetHeaderAsync( user.Email);

        // The user has already logged all of their period days for the current cycle, so the predicted period should be for the next 3 months
        var response = await _client.GetAsync("/cycle/predicted-period");
        var actual = await response.Content.ReadFromJsonAsync<List<DateTimeOffset>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual.Should().HaveCount((int)Math.Floor(user.PeriodLength!.Value) * 3);

        var startDate = _dbContext.Cycles.First(c => c.Id == user.CurrentCycleId).StartDate.Date;
        AssertFutureCyclePredictions(0, (int)Math.Floor(user.PeriodLength!.Value), startDate, actual);
        
        await ClearToken();
    }
    
    [Fact]
    public async Task Get_Period_Cycle_Stats_Unauthorized()
    {
        await ClearToken();
        var response = await _client.GetAsync("/cycle/period-cycle-stats");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Get_Period_Cycle_Stats_Success()
    {
        var user = _userManager.Users.First(u => u.Email == "user2@application");
        var cycle = _dbContext.Cycles.First(c => c.UserId == user.Id);
        await AuthorizeUserAndSetHeaderAsync( user.Email);

        var response = await _client.GetAsync("/cycle/period-cycle-stats");
        var actual = await response.Content.ReadFromJsonAsync<PeriodCycleStatsDto>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual.Should().NotBeNull();
        actual!.AverageCycleLength.Should().Be((int) Math.Floor(user.CycleLength!.Value));
        actual.AveragePeriodLength.Should().Be((int) Math.Floor(user.PeriodLength!.Value));
        actual.CurrentCycleLength.Should().Be(DateTime.Now.Subtract(cycle.StartDate.Date).Days);
        
        await ClearToken();
    }
    
    [Fact]
    public async Task Set_Period_Cycle_Length_Unauthorized()
    {
        await ClearToken();
        var response = await _client.PostAsJsonAsync("/cycle/period-cycle-length", new PeriodAndCycleLengthDto());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Set_Period_Cycle_Length_Success()
    {
        var user = _userManager.Users.First(u => u.Email == "user2@application");
        await AuthorizeUserAndSetHeaderAsync( user.Email);

        var response = await _client.PostAsJsonAsync("/cycle/period-cycle-length", new PeriodAndCycleLengthDto
        {
            CycleLength = 30,
            PeriodLength = 5
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        user = _userManager.Users.First(u => u.Email == "user2@application");
        
        _dbContext.Entry(user).Reload();
        
        user.CycleLength.Should().Be(30);
        user.PeriodLength.Should().Be(5);
        
        user = _userManager.Users.First(u => u.Email == "user2@application");
        user.CycleLength = 28; // Reset so the rest of the tests don't fail
        await _userManager.UpdateAsync(user);
        await ClearToken();
    }

    [Fact]
    public async Task Set_Period_Negative_Period_Length()
    {
        var user = _userManager.Users.First(u => u.Email == "user2@application");
        await AuthorizeUserAndSetHeaderAsync( user.Email);
        
        var response = await _client.PostAsJsonAsync("/cycle/period-cycle-length", new PeriodAndCycleLengthDto
        {
            CycleLength = 30,
            PeriodLength = -1
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        user = _userManager.Users.First(u => u.Email == "user2@application");
        user.CycleLength.Should().Be(28);
        user.PeriodLength.Should().Be(5);
    }
    
    [Fact]
    public async Task Set_Period_TooHigh_Period_Length()
    {
        var user = _userManager.Users.First(u => u.Email == "user2@application");
        await AuthorizeUserAndSetHeaderAsync( user.Email);
        
        var response = await _client.PostAsJsonAsync("/cycle/period-cycle-length", new PeriodAndCycleLengthDto
        {
            CycleLength = 30,
            PeriodLength = 101
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        user = _userManager.Users.First(u => u.Email == "user2@application");
        user.CycleLength.Should().Be(28);
        user.PeriodLength.Should().Be(5);
    }
    
    [Fact]
    public async Task Set_Period_Negative_Cycle_Length()
    {
        var user = _userManager.Users.First(u => u.Email == "user2@application");
        await AuthorizeUserAndSetHeaderAsync( user.Email);
        
        var response = await _client.PostAsJsonAsync("/cycle/period-cycle-length", new PeriodAndCycleLengthDto
        {
            CycleLength = -1,
            PeriodLength = 5
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        user = _userManager.Users.First(u => u.Email == "user2@application");
        user.CycleLength.Should().Be(28);
        user.PeriodLength.Should().Be(5);
    }
    
    [Fact]
    public async Task Set_Period_TooHigh_Cycle_Length()
    {
        var user = _userManager.Users.First(u => u.Email == "user2@application");
        await AuthorizeUserAndSetHeaderAsync( user.Email);
        
        var response = await _client.PostAsJsonAsync("/cycle/period-cycle-length", new PeriodAndCycleLengthDto
        {
            CycleLength = 101,
            PeriodLength = 5
        });
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        user = _userManager.Users.First(u => u.Email == "user2@application");
        user.CycleLength.Should().Be(28);
        user.PeriodLength.Should().Be(5);
    }
    
    #region Utilities
    private void AssertFutureCyclePredictions(int startingArrayIndex, int periodLength, DateTime startDate,
        List<DateTimeOffset> predictedDayList)
    {
        for (var j = 1; j < 4; j++)
        {
            for (var i = 0; i < periodLength; i++)
            {
                predictedDayList[startingArrayIndex].Date.Should().Be(startDate.AddDays((28 * j) + i));
                startingArrayIndex++;
            }
        }
    }
    #endregion
}
