using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Dapper;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models.Dto.Identity;
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
        var response = await _client.PostAsync($"/Metric?dateTimeOffsetString={date}", null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

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
            .Where(cdm => cdm.CalendarDay.Date == utcDate
                          && cdm.CalendarDay.UserId == user.Id)
            .ToList();

        var loginRequestDto = new LoginRequestDto()
        {
            Email = user.Email,
            Password = "P@ssw0rd.+"
        };

        var response = await _client.PostAsJsonAsync("/identity/auth/login", loginRequestDto);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        if (authResponse != null)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authResponse.Token}");
        }
        

        // Act
        response = await _client.GetAsync($"/Metric/{date}");
        
        // print response
        var content = await response.Content.ReadAsStringAsync();
        _testOutputHelper.WriteLine(JsonConvert.SerializeObject(content, Formatting.Indented));
        var actual = await response.Content.ReadFromJsonAsync<ICollection<CalendarDayMetric>>();
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        actual.Should().BeEquivalentTo(expected);
    }
}
