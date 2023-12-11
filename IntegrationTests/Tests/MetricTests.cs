using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Days;
using Models.Dto.Metrics;
using Models.Identity;
using Models.Util;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class MetricTests(VitalApiFactory vaf) : TestBase(vaf)
{
    [Fact]
    public async Task Get_Should_be_unauthorized()
    {
        await ClearToken();

        var date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var response = await _client.GetAsync($"/Metric/{date}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UploadMetricForADay_Should_be_unauthorized()
    {
        await ClearToken();

        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = Guid.NewGuid(),
            MetricsId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        var response = await _client.PostAsJsonAsync($"/Metric", metricRegisterMetricDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_Should_return_a_list_of_metrics()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        var date = DateTimeOffset.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var utcDate = DateTime.Parse(date, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

        var expected = _dbContext.CalendarDayMetric
            .Include(cdm => cdm.Metrics)
            .Include(cdm => cdm.MetricValue)
            .Where(cdm => cdm.CalendarDay!.Date.Date == utcDate.Date
                          && cdm.CalendarDay.UserId == user.Id)
            .ToList();
        await AuthorizeUserAndSetHeaderAsync(user.Email!);

        // Act
        var response = await _client.GetAsync($"/Metric/{date}");
        var actual = await response.Content.ReadFromJsonAsync<ICollection<CalendarDayMetric>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual?.Count.Should().Be(expected.Count);

        // Cleanup
        await ClearToken();
    }

    [Fact]
    [Description("Logging flow should mark the day as a period day.")]
    public async Task Save_Flow_Marks_Day_As_Period_Day()
    {
        try
        {
            // Arrange
            var user = await RegisterUserAndCreateCycle("temp@application");
            await AuthorizeUserAndSetHeaderAsync(user.Email);
            var metricRegisterMetricDto = await GetRegisterMetricDtoFlow();

            // Act
            var response = await _client.PostAsJsonAsync($"/Metric",
                new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var cycleDay = await _dbContext.CycleDays.Where(cd => cd.UserId == user.Id)
                .OrderByDescending(cd => cd.Date)
                .FirstAsync();
            cycleDay.IsPeriod.Should().BeTrue();
        } finally
        {
            await Cleanup("temp@application");
        }
    }

    [Fact]
    public async Task Upload_Metrics_BadRequest_Wrong_MetricID()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await AuthorizeUserAndSetHeaderAsync(user.Email);

        var date = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = Guid.NewGuid(),
            MetricsId = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/Metric",
            new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Cleanup
        await ClearToken();
    }

    [Fact]
    public async Task SaveMetrics_ThrowsException_FutureDate()
    {
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await AuthorizeUserAndSetHeaderAsync(user.Email);
        var futureDate = DateTimeOffset.UtcNow.AddDays(2); // A future date

        var metricRegisterMetricDto = await GetRegisterMetricDtoFlow(futureDate);

        // Act
        var response =
            await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails?.Title.Should().Be("Bad Request");
        problemDetails?.Detail.Should().Contain("Cannot log metrics for a future date.");

        // Cleanup
        await ClearToken();
    }

    [Fact]
    public async Task SaveMetrics_No_Historic_Data_Success()
    {
        await ClearToken();
        // Arrange
        var user = await _dbContext.Users.FirstAsync(u => u.Email == "user@application");
        await AuthorizeUserAndSetHeaderAsync(user.Email);

        // Date before existing cycle
        var dateBeforeCurrentCycle = DateTimeOffset.UtcNow.AddMonths(-2);

        // Adding the Flow metric triggers creation of new cycle.
        var metricRegisterMetricDto = await GetRegisterMetricDtoFlow(dateBeforeCurrentCycle);

        // Act
        var response =
            await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });

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
        await ClearToken();
    }

    [Fact]
    public async Task SaveMetrics_Historic_Data_Success()
    {
        try
        {
            // Arrange
            var user = await RegisterUserAndCreateCycle("temp@application");
            await AuthorizeUserAndSetHeaderAsync(user.Email!);

            // Add two historic cycles
            var dates = new List<DateTimeOffset>
            {
                DateTimeOffset.UtcNow.AddDays(-70),
                DateTimeOffset.UtcNow.AddDays(-30)
            };

            var historicCycles = await CreateHistoricCycles(dates, user.Id);
            _dbContext.Cycles.AddRange(historicCycles);
            await _dbContext.SaveChangesAsync();
            
            var currentCycleCount = await _dbContext.Cycles.Where(c => c.UserId == user.Id).CountAsync();
            var metricRegisterMetricDto = await GetRegisterMetricDtoFlow(historicCycles[0].StartDate.AddDays(15));

            // Act
            var response =
                await _client.PostAsJsonAsync($"/Metric",
                    new List<MetricRegisterMetricDto> { metricRegisterMetricDto });
            _dbContext.ChangeTracker.Clear();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check that the new cycle successfully shifted the start and end dates of other cycles
            var cycles = await _dbContext.Cycles
                .Where(c => c.UserId == user.Id)
                .OrderBy(c => c.StartDate)
                .ToListAsync();
            cycles.Count.Should().Be(currentCycleCount + 1);
            for (var i = 0;
                 i < cycles.Count - 1;
                 i++) // -1 because the last cycle is the current cycle, and has no end date
            {
                cycles[i].EndDate!.Value.Date.AddDays(1).Should().Be(cycles[i + 1].StartDate.Date);
            }

            cycles[^1].EndDate.Should().BeNull();
        } finally
        {
            await Cleanup("temp@application");
        }
    }

    [Fact]
    public async Task SaveMetrics_No_Current_Cycle()
    {
        try
        {
            await RegisterNewUserAndVerifyEmailAsync("temp@application");
            await AuthorizeUserAndSetHeaderAsync("temp@application");

            var registerMetricDto = await GetRegisterMetricDtoFlow();
            var response =
                await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { registerMetricDto });
            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            problemDetails?.Title.Should().Be("Bad Request");
            problemDetails?.Detail.Should().Contain("Cannot log metrics without a current cycle.");
        } finally
        {
            await Cleanup("temp@application");
        }
    }

    [Fact]
    [Description(
        "If a user tries to log a flow within seven days of the start of another cycle, the cycle should be extended instead of creating a new very short cycle.")]
    public async Task Save_Flow_Within_Seven_Days_Of_Another_Cycle()
    {
        try
        {
            // Arrange
            var user = await RegisterUserAndCreateCycle("temp@application");
            var cycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id);
            var dateBeforeCurrentCycle = cycle.StartDate.AddDays(-7);
            var metricRegisterMetricDto = await GetRegisterMetricDtoFlow(dateBeforeCurrentCycle);
            await AuthorizeUserAndSetHeaderAsync("temp@application");

            // Act
            var response =
                await _client.PostAsJsonAsync($"/Metric",
                    new List<MetricRegisterMetricDto> { metricRegisterMetricDto });
            _dbContext.ChangeTracker.Clear();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check that the cycle was extended
            (await _dbContext.Cycles.Where(c => c.UserId == user.Id).CountAsync()).Should().Be(1);
            var updatedCycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id);
            updatedCycle.StartDate.Date.Should().Be(dateBeforeCurrentCycle.Date);
            updatedCycle.EndDate.Should().BeNull();
        } finally
        {
            await Cleanup("temp@application");
        }
    }

    [Fact]
    [Description(
        "If a user tries to log a flow more than seven days before the start of another cycle, a new cycle should be created.")]
    public async Task Save_Flow_Eight_Days_Before_Another_Cycle()
    {
        // Arrange
        var user = await RegisterUserAndCreateCycle("temp@application");
        var cycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id);
        var dateBeforeCurrentCycle = cycle.StartDate.AddDays(-20);
        var metricRegisterMetricDto = await GetRegisterMetricDtoFlow(dateBeforeCurrentCycle);
        await AuthorizeUserAndSetHeaderAsync("temp@application");

        // Act
        var response =
            await _client.PostAsJsonAsync($"/Metric", new List<MetricRegisterMetricDto> { metricRegisterMetricDto });
        _dbContext.ChangeTracker.Clear();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Check that a new cycle was created
        (await _dbContext.Cycles.Where(c => c.UserId == user.Id).CountAsync()).Should().Be(2);
        var newCycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id && c.EndDate != null);
        newCycle.StartDate.Date.Should().Be(dateBeforeCurrentCycle.Date);
        newCycle.EndDate.Should().NotBeNull();

        // Check that current cycle was not changed
        var currentCycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id && c.EndDate == null);
        currentCycle.StartDate.Date.Should().Be(cycle.StartDate.Date);
        currentCycle.EndDate.Should().BeNull();

        // Cleanup
        await ClearToken();
        await RemoveUserAsync("temp@application");
    }

    [Fact]
    [Description("If a user logs a non-flow metric before an existing cycle, the following cycle should be extended.")]
    public async Task Logging_Cramps_Should_Extend_Following_Cycle()
    {
        try
        {
            // Arrange
            var user = await RegisterUserAndCreateCycle("temp@application");
            await AuthorizeUserAndSetHeaderAsync("temp@application");
            var cycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id);
            var dateBeforeCurrentCycle = cycle.StartDate.AddDays(-7);

            var metricsId = (await _dbContext.Metrics.FirstAsync(m => m.Name == "Cramps")).Id;
            var metricRegisterMetricDto = new MetricRegisterMetricDto()
            {
                CreatedAt = dateBeforeCurrentCycle,
                MetricsId = metricsId
            };

            // Act
            var response =
                await _client.PostAsJsonAsync($"/Metric",
                    new List<MetricRegisterMetricDto> { metricRegisterMetricDto });
            _dbContext.ChangeTracker.Clear();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check that the cycle was extended
            (await _dbContext.Cycles.Where(c => c.UserId == user.Id).CountAsync()).Should().Be(1);
            var updatedCycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id);
            updatedCycle.StartDate.Date.Should().Be(dateBeforeCurrentCycle.Date);
            updatedCycle.EndDate.Should().BeNull();
        } finally
        {
            await Cleanup("temp@application");
        }
    }

    [Fact]
    [Description(
        "If a user logs a non-flow metric before an existing cycle, the following cycle should be extended, but if user then logs flow before that," +
        "a new cycle should be created and it should encapsulate the non-flow metric.")]
    public async Task Logging_Flow_Should_Update_Metric_Cycle_Link()
    {
        try
        {
            // Arrange
            var user = await RegisterUserAndCreateCycle("temp@application");
            await AuthorizeUserAndSetHeaderAsync("temp@application");
            var cycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id);
            var crampsMetricLogDate = cycle.StartDate.AddDays(-7);

            var calendarDay = new CycleDay()
            {
                Id = Guid.NewGuid(),
                CycleId = cycle.Id,
                Date = crampsMetricLogDate,
                IsPeriod = false,
                UserId = user.Id
            };
            _dbContext.CycleDays.Add(calendarDay);

            var metric = await _dbContext.Metrics.FirstAsync(m => m.Name == "Cramps");
            var calendarDayMetric = new CalendarDayMetric()
            {
                Id = Guid.NewGuid(),
                CalendarDayId = calendarDay.Id,
                CreatedAt = crampsMetricLogDate,
                MetricsId = metric.Id
            };
            _dbContext.CalendarDayMetric.Add(calendarDayMetric);
            await _dbContext.SaveChangesAsync();

            var flowMetricLogDate = cycle.StartDate.AddDays(-14);
            var flowMetricRegisterMetricDto = await GetRegisterMetricDtoFlow(flowMetricLogDate);
            // Act
            var response = await _client.PostAsJsonAsync("/Metric",
                new List<MetricRegisterMetricDto>() { flowMetricRegisterMetricDto });
            _dbContext.ChangeTracker.Clear();
            var newlyCreatedCycle = await _dbContext.Cycles.FirstAsync(c => c.UserId == user.Id && c.EndDate != null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await _dbContext.Cycles.Where(c => c.UserId == user.Id).CountAsync()).Should().Be(2);
            newlyCreatedCycle.StartDate.Date.Should().Be(cycle.StartDate.Date.AddDays(-14));
            newlyCreatedCycle.EndDate!.Value.Date.Should().Be(cycle.StartDate.Date.AddDays(-1));

            calendarDay =
                await _dbContext.CycleDays
                    .Include(cycleDay => cycleDay.Cycle!)
                    .FirstAsync(cd =>
                        cd.UserId == user.Id && cd.Date.UtcDateTime.Date == crampsMetricLogDate.UtcDateTime.Date);
            calendarDay.Cycle!.Id.Should().Be(newlyCreatedCycle.Id);
        } finally
        {
            await Cleanup("temp@application");
        }
    }


    #region Utility Methods

    private async Task<MetricRegisterMetricDto> GetRegisterMetricDtoFlow(DateTimeOffset? createdAt = null)
    {
        createdAt ??= DateTimeOffset.UtcNow;
        var flowMetric = await _dbContext.Metrics.FirstAsync(m => m.Name.Contains("Flow"));
        var metricValue = await _dbContext.MetricValue.FirstAsync(m => m.MetricsId == flowMetric.Id);
        var metricRegisterMetricDto = new MetricRegisterMetricDto()
        {
            MetricValueId = metricValue.Id,
            MetricsId = flowMetric.Id,
            CreatedAt = createdAt.Value.UtcDateTime
        };
        return metricRegisterMetricDto;
    }

    private async Task<ApplicationUser> RegisterUserAndCreateCycle(string email, DateTimeOffset? cycleStartDate = null)
    {
        cycleStartDate ??= DateTimeOffset.UtcNow.AddDays(-1);
        var user = await RegisterNewUserAndVerifyEmailAsync(email);
        var flow = await GetRegisterMetricDtoFlow();
        var cycleDayId = Guid.NewGuid();
        var cycle = new Cycle()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            StartDate = cycleStartDate.Value.UtcDateTime,
            EndDate = null,
            CycleDays = new List<CycleDay>
            {
                new CycleDay
                {
                    Id = cycleDayId,
                    UserId = user.Id,
                    Date = cycleStartDate.Value.UtcDateTime,
                    IsPeriod = true,
                    SelectedMetrics = new List<CalendarDayMetric>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = cycleStartDate,
                            CalendarDayId = cycleDayId,
                            MetricValueId = flow.MetricValueId,
                            MetricsId = flow.MetricsId
                        }
                    }
                }
            }
        };
        _dbContext.Cycles.Add(cycle);
        user.CurrentCycleId = cycle.Id;
        await _dbContext.SaveChangesAsync();
        await _userManager.UpdateAsync(user);
        return user;
    }

    private async Task Cleanup(string email)
    {
        await ClearToken();
        await RemoveUserAsync(email);
    }

    private async Task<List<Cycle>> CreateHistoricCycles(IReadOnlyList<DateTimeOffset> startDateList, Guid userId)
{
    var metric = await _dbContext.Metrics.FirstAsync(m => m.Name.Contains("Flow"));
    var calendarDayIds = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() };
    var historicCycles = new List<Cycle>()
    {
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StartDate = startDateList[0],
            EndDate = startDateList[1].AddDays(-1),
            CycleDays = new List<CycleDay>
            {
                new CycleDay
                {
                    Id = calendarDayIds[0],
                    UserId = userId,
                    Date = startDateList[0],
                    IsPeriod = true,
                    SelectedMetrics = new List<CalendarDayMetric>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = startDateList[0],
                            CalendarDayId = calendarDayIds[0],
                            MetricsId = metric.Id
                        }
                    }
                }
            }
        },
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StartDate = startDateList[1],
            EndDate = DateTimeOffset.UtcNow.AddDays(-2), // the current cycle starts at -1 for users created with utility methods
            CycleDays = new List<CycleDay>
            {
                new()
                {
                    Id = calendarDayIds[1],
                    UserId = userId,
                    Date = startDateList[1],
                    IsPeriod = true,
                    SelectedMetrics = new List<CalendarDayMetric>()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            CreatedAt = startDateList[1],
                            CalendarDayId = calendarDayIds[1],
                            MetricsId = metric.Id
                        }
                    }
                }
            }
        }
    };
    return historicCycles;
}

    #endregion
}
