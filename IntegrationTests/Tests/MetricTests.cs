using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;
using Models.Dto.Metrics;
using Models.Util;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class MetricTests(VitalApiFactory vaf) : TestBase(vaf)
{
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
            .Where(cdm => cdm.CalendarDay!.Date.Date == utcDate.Date
                          && cdm.CalendarDay.UserId == user.Id)
            .ToList();
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email!);

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
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user3@application");
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email!);
        _dbContext.Cycles.RemoveRange(_dbContext.Cycles.Where(c => c.UserId == user.Id && c.EndDate == null));
        await _dbContext.SaveChangesAsync();

        var metric = await _dbContext.Metrics.FirstAsync(m => m.Name != "Flow");
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
        await Utilities.AuthorizeUserAndSetHeaderAsync(_client, user.Email!);

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
        await Utilities.ClearToken(_client);
    }
}
