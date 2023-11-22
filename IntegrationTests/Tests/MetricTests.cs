using System.Net;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class MetricTests
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    public MetricTests(VitalApiFactory waf)
    {
        _client = waf.Client;
        _dbContext = waf.DbContext;
    }
    
    [Fact]
    public async Task Get_Should_be_unauthorized()
    {
        var date = DateTimeOffset.Now;
        var response = await _client.GetAsync($"/Metric/{date}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
