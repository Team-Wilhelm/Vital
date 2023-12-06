using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Setup;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class PeriodTests(VitalApiFactory vaf) : TestBase(vaf)
{
    [Fact]
    public async Task Get_Predicted_Period_Should_be_unauthorized()
    {
        await Utilities.ClearToken(_client);

        var response = await _client.GetAsync($"/cycle/predicted-period");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_Predicted_Period_OnGoing_Period_Success()
    {
        var user = _userManager.Users.First(u => u.Email == "user2@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);

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
        
        await Utilities.ClearToken(_client);
    }

    [Fact]
    public async Task Get_Predicted_Period_Future_Success()
    {
        var user = _userManager.Users.First(u => u.Email == "user@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);

        // The user has already logged all of their period days for the current cycle, so the predicted period should be for the next 3 months
        var response = await _client.GetAsync("/cycle/predicted-period");
        var actual = await response.Content.ReadFromJsonAsync<List<DateTimeOffset>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual.Should().HaveCount((int)Math.Floor(user.PeriodLength!.Value) * 3);

        var startDate = _dbContext.Cycles.First(c => c.Id == user.CurrentCycleId).StartDate.Date;
        AssertFutureCyclePredictions(0, (int)Math.Floor(user.PeriodLength!.Value), startDate, actual);
        
        await Utilities.ClearToken(_client);
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
