using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Dto.Cycle;
using Models.Dto.Identity;
using Models.Dto.Metrics;
using Models.Identity;
using Models.Responses;
using Models.Util;
using Newtonsoft.Json;
using Vital.Models.Exception;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class MetricTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceScope _scope;

    public MetricTests(VitalApiFactory waf, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _client = waf.Client;
        _scope = waf.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task Get_Should_be_unauthorized()
    {
        await Utilities.ClearToken(_client);

        var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var response = await _client.GetAsync($"/Metric/{date}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task UploadMetricForADay_Should_be_unauthorized()
    {
        await Utilities.ClearToken(_client);

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
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);

        // Act
        var response = await _client.GetAsync($"/Metric/{date}");
        var actual = await response.Content.ReadFromJsonAsync<ICollection<CalendarDayMetric>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual?.Count.Should().Be(expected.Count);
        
        // Cleanup
        await Utilities.ClearToken(_client);
    }

    [Fact]
    public async Task Upload_Metrics_Success()
    {
        await Utilities.ClearToken(_client);
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);
        var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        _dbContext.Cycles.RemoveRange(_dbContext.Cycles.Where(c => c.UserId == user.Id && c.EndDate == null));
        await _dbContext.SaveChangesAsync();
        
        var metric = await _dbContext.Metrics.FirstAsync();
        var metricValue = await _dbContext.MetricValue.FirstAsync(m => m.MetricsId == metric.Id);
        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = metricValue.Id,
            MetricsId = metric.Id,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric",
            new List<MetricRegisterMetricDto> { metricRegisterMetricDto });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Cleanup
        await Utilities.ClearToken(_client);
    }

    [Fact]
    public async Task Upload_Metrics_BadRequest_Wrong_MetricID()
    {
        await Utilities.ClearToken(_client);
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);

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
        _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        // Cleanup
        await Utilities.ClearToken(_client);
    }
    
    [Fact]
    public async Task SaveMetrics_ThrowsException_FutureDate()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);
        var futureDate = DateTime.UtcNow.AddDays(2); // A future date

        var metric = await _dbContext.Metrics.FirstAsync();
        var metricValue = await _dbContext.MetricValue.FirstAsync(m => m.MetricsId == metric.Id);
        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = metricValue.Id,
            MetricsId = metric.Id,
            CreatedAt = futureDate
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be("Bad Request");
        problemDetails?.Detail.Should().Contain("Cannot log metrics for a future date.");

        // Cleanup
        await Utilities.ClearToken(_client);
    }
    
    [Fact]
    public async Task SaveMetrics_No_Historic_Data_Success()
    {
        await Utilities.ClearToken(_client);
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);
        
        // Date before existing cycle
        var dateBeforeCurrentCycle = DateTime.UtcNow.AddMonths(-2); 

        // Adding the Flow metric triggers creation of new cycle.
        var flowMetric = await _dbContext.Metrics.FirstAsync(m => m.Name.Contains("Flow")); 
        var metricValue = await _dbContext.MetricValue.FirstAsync(m => m.MetricsId == flowMetric.Id);
        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = metricValue.Id,
            MetricsId = flowMetric.Id,
            CreatedAt = dateBeforeCurrentCycle
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check that a new cycle was created before the current cycle
        var currentCycleId = await _dbContext.Users
            .Where(u => u.Id == user.Id)
            .Select(u => u.CurrentCycleId)
            .FirstOrDefaultAsync();
        var currentCycleStartDate = await _dbContext.Cycles
            .Where(c => c.UserId == user.Id && c.EndDate == null)
            .Select(c => c.StartDate)
            .FirstOrDefaultAsync();
        var previousCycle = await _dbContext.Cycles
            .Where(c => c.UserId == user.Id && c.EndDate <= currentCycleStartDate)
            .OrderByDescending(c => c.EndDate)
            .FirstOrDefaultAsync();
        
        previousCycle.Should().NotBeNull();
        Debug.Assert(user.CurrentCycleId != null, "user.CurrentCycleId != null");
        previousCycle?.Id.Should().NotBe((Guid)user.CurrentCycleId); //User always has a current cycle

        // Cleanup
        await Utilities.ClearToken(_client);
    }
    
    [Fact]
    public async Task SaveMetrics_Historic_Data_Success()
    {
        await Utilities.ClearToken(_client);
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);
        
        // Date before current cycle
        var dateBeforeCurrentCycle = DateTime.UtcNow.AddMonths(-2); 

        // Create a historic cycle
        var historicFlowMetric = await _dbContext.Metrics.FirstAsync(m => m.Name.Contains("Flow")); 
        var historicMetricValue = await _dbContext.MetricValue.FirstAsync(m => m.MetricsId == historicFlowMetric.Id);
        var historicMetricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = historicFlowMetric.Id,
            MetricsId = historicMetricValue.Id,
            CreatedAt = dateBeforeCurrentCycle
        };
        
        // Create a cycle that starts after the start of the historic cycle and before the current cycle
        var flowMetric = await _dbContext.Metrics.FirstAsync(m => m.Name.Contains("Flow"));
        var metricValue = await _dbContext.MetricValue.FirstAsync(m => m.MetricsId == flowMetric.Id);
        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = metricValue.Id,
            MetricsId = flowMetric.Id,
            CreatedAt = dateBeforeCurrentCycle.AddMonths(1)
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check that a new cycle was created before the current cycle
        // get historic cycle id based on date
        var historicCycleId = await _dbContext.Cycles
            .Where(c => c.UserId == user.Id && c.StartDate == dateBeforeCurrentCycle)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();
        var currentCycleId = user.CurrentCycleId;
        // get cycle that starts after the historic cycle and before the current cycle
        var newCycle = await _dbContext.Cycles
            .Where(c => c.UserId == user.Id && c.StartDate > dateBeforeCurrentCycle && c.StartDate < DateTime.UtcNow)
            .OrderByDescending(c => c.EndDate)
            .FirstOrDefaultAsync();
        
        newCycle.Should().NotBeNull();
        newCycle?.Id.Should().NotBe(historicCycleId);
        //TODO why does current cycle have the same id as new cycle?
        newCycle?.Id.Should().NotBe((Guid)currentCycleId!); //User always has a current cycle

        // Cleanup
        await Utilities.ClearToken(_client);
    }
}
