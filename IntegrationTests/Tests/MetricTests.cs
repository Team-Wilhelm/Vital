using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Dto.Cycle;
using Models.Dto.Identity;
using Models.Dto.Metrics;
using Models.Responses;
using Models.Util;
using Newtonsoft.Json;
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
        var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var response = await _client.GetAsync($"/Metric/{date}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadMetricForADay_Should_be_unauthorized()
    {
        var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = Guid.NewGuid(),
            MetricsId = Guid.NewGuid()
        };
        var response = await _client.PostAsJsonAsync($"/Metric?dateTimeOffsetString={date}", metricRegisterMetricDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    //TODO: Update this test, after we decide how to handle the metrics in the calendar
    [Fact]
    public async Task Get_Should_return_a_list_of_metrics()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync();
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

        await Utilities.ClearToken(_client);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual?.Count.Should().Be(expected.Count);
    }

    [Fact]
    public async Task Upload_Metrics_Success()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync();
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);

        var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        var metric = await _dbContext.Metrics.FirstAsync();
        var metricValue = await _dbContext.MetricValue.FirstAsync(m => m.MetricsId == metric.Id);

        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = metricValue.Id,
            MetricsId = metric.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric?date={date}",
            new List<MetricRegisterMetricDto> { metricRegisterMetricDto });
        var actual = await response.Content.ReadFromJsonAsync<CalendarDayDto>();
        
        await Utilities.ClearToken(_client);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual.Should().NotBeNull();
        // actual?.SelectedMetrics.Count.Should().Be(1); //TODO: Put this in code first, currently not implemented
        //actual?.SelectedMetrics.First().MetricValueId.Should().Be(metricValue.Id);
        //actual?.SelectedMetrics.First().MetricsId.Should().Be(metric.Id);
        actual?.Date.Date.Should()
            .Be(DateTime.Parse(date, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).Date);
        actual?.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Upload_Metrics_BadRequest_Wrong_MetricID()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync();
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email);

        var date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = Guid.NewGuid(),
            MetricsId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric?dateTimeOffsetString={date}",
            new List<MetricRegisterMetricDto> { metricRegisterMetricDto });
        _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
        
        await Utilities.ClearToken(_client);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
