using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Dto.Metrics;
using Models.Pagination;
using Models.Util;
using Newtonsoft.Json;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class MetricTests(VitalApiFactory vaf) : TestBase(vaf)
{
    [Fact]
    public async Task Get_Should_be_unauthorized()
    {
        await ClearToken(_client);

        var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var response = await _client.GetAsync($"/Metric/{date}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadMetricForADay_Should_be_unauthorized()
    {
        await ClearToken(_client);

        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = Guid.NewGuid(),
            MetricsId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        var response = await _client.PostAsJsonAsync($"/Metric", metricRegisterMetricDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_Should_return_a_list_of_metrics()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        var date = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var utcDate = DateTime.Parse(date, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

        var expected = _dbContext.CalendarDayMetric
            .Include(cdm => cdm.Metrics)
            .Include(cdm => cdm.MetricValue)
            .Where(cdm => cdm.CalendarDay.Date.Date == utcDate.Date
                          && cdm.CalendarDay.UserId == user.Id)
            .ToList();
        await AuthorizeUserAndSetHeaderAsync(_client, user.Email);

        // Act
        var response = await _client.GetAsync($"/Metric/{date}");
        var actual = await response.Content.ReadFromJsonAsync<ICollection<CalendarDayMetric>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual?.Count.Should().Be(expected.Count);

        // Cleanup
        await ClearToken(_client);
    }

    [Fact]
    public async Task Upload_Metrics_Success()
    {
        await ClearToken(_client);
        
        // Arrange
        var user = await RegisterNewUserAndVerifyEmailAsync("temp@application");
        var cycle = new Cycle()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            StartDate = DateTimeOffset.UtcNow.AddDays(-1),
            EndDate = null
        };
        _dbContext.Cycles.Add(cycle);
        user.CurrentCycleId = cycle.Id;
        await _dbContext.SaveChangesAsync();
        await _userManager.UpdateAsync(user);
        
        await AuthorizeUserAndSetHeaderAsync(_client, user.Email);
        var metricRegisterMetricDto = await GetRegisterMetricDtoFlow();

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric",
            new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Cleanup
        await ClearToken(_client);
        await RemoveUserAsync("temp@application");
    }

    [Fact]
    public async Task Upload_Metrics_BadRequest_Wrong_MetricID()
    {
        await ClearToken(_client);
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await AuthorizeUserAndSetHeaderAsync(_client, user.Email);

        var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = Guid.NewGuid(),
            MetricsId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric",
            new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        // Cleanup
        await ClearToken(_client);
    }
    
    [Fact]
    public async Task SaveMetrics_ThrowsException_FutureDate()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await AuthorizeUserAndSetHeaderAsync(_client, user.Email);
        var futureDate = DateTime.UtcNow.AddDays(2); // A future date
        
        var metricRegisterMetricDto = await GetRegisterMetricDtoFlow(futureDate);

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be("Bad Request");
        problemDetails?.Detail.Should().Contain("Cannot log metrics for a future date.");

        // Cleanup
        await ClearToken(_client);
    }
    
    [Fact]
    public async Task SaveMetrics_No_Historic_Data_Success()
    {
        await ClearToken(_client);
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await AuthorizeUserAndSetHeaderAsync(_client, user.Email);
        
        // Date before existing cycle
        var dateBeforeCurrentCycle = DateTime.UtcNow.AddMonths(-2); 

        // Adding the Flow metric triggers creation of new cycle.
        var metricRegisterMetricDto = await GetRegisterMetricDtoFlow(dateBeforeCurrentCycle);
        
        // Act
        var response = await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check that a new cycle was created before the current cycle
        var currentCycleStartDate = await _dbContext.Cycles
            .Where(c => c.UserId == user.Id && c.EndDate == null)
            .Select(c => c.StartDate)
            .FirstOrDefaultAsync();
        var previousCycle = await _dbContext.Cycles
            .Where(c => c.UserId == user.Id && c.EndDate <= currentCycleStartDate)
            .OrderByDescending(c => c.EndDate)
            .FirstOrDefaultAsync();
        
        previousCycle.Should().NotBeNull();
        previousCycle?.Id.Should().NotBe((Guid)user.CurrentCycleId!); //User always has a current cycle

        // Cleanup
        await ClearToken(_client);
    }
    
    [Fact]
    public async Task SaveMetrics_Historic_Data_Success()
    {
        await ClearToken(_client);
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await AuthorizeUserAndSetHeaderAsync(_client, user.Email);
        
        // Date before current cycle
        var dateBeforeCurrentCycle = DateTime.UtcNow.AddMonths(-2); 

        // Create and post a historic cycle
        var historicMetricRegisterMetricDto = await GetRegisterMetricDtoFlow(dateBeforeCurrentCycle);
        await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { historicMetricRegisterMetricDto });
        
        // Create a cycle that starts after the start of the historic cycle and before the current cycle
        var metricRegisterMetricDto = await GetRegisterMetricDtoFlow(dateBeforeCurrentCycle.AddMonths(1));
        var currentCycleCount = await _dbContext.Cycles
            .Where(c => c.UserId == user.Id)
            .CountAsync();

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Check that the new cycle successfully shifted the start and end dates of other cycles
        var cycles = await _dbContext.Cycles
            .Where(c => c.UserId == user.Id)
            .OrderBy(c => c.StartDate)
            .ToListAsync();
        cycles.Count.Should().Be(currentCycleCount + 1);
        for (var i = 0; i < cycles.Count - 1; i++) // -1 because the last cycle is the current cycle, and has no end date
        {
            cycles[i].EndDate!.Value.Date.AddDays(1).Should().Be(cycles[i + 1].StartDate.Date);
        }
        
        cycles[^1].EndDate.Should().BeNull();

        // Cleanup
        await ClearToken(_client);
    }

    [Fact]
    public async Task SaveMetrics_No_Current_Cycle()
    {
        var user = RegisterNewUserAndVerifyEmailAsync("temp@application");
        await AuthorizeUserAndSetHeaderAsync(_client, "temp@application");
        
        var registerMetricDto = await GetRegisterMetricDtoFlow();
        var response = await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { registerMetricDto });
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        problemDetails?.Title.Should().Be("Bad Request");
        problemDetails?.Detail.Should().Contain("Cannot log metrics without a current cycle.");
        
        await RemoveUserAsync("temp@application");
    }

    private async Task<MetricRegisterMetricDto> GetRegisterMetricDtoFlow(DateTimeOffset? createdAt = null)
    {
        createdAt ??= DateTimeOffset.UtcNow;
        var flowMetric = await _dbContext.Metrics.FirstAsync(m => m.Name.Contains("Flow")); 
        var metricValue = await _dbContext.MetricValue.FirstAsync(m => m.MetricsId == flowMetric.Id);
        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = metricValue.Id,
            MetricsId = flowMetric.Id,
            CreatedAt = createdAt.Value
        };
        return metricRegisterMetricDto;
    } 
}
