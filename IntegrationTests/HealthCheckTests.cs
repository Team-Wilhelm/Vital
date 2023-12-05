using System.Net.Http.Json;
using IntegrationTests.Setup;

namespace IntegrationTests;

[Collection("VitalApi")]
public class HealthCheckTests
{
    private readonly HttpClient _client;

    public HealthCheckTests(VitalApiFactory waf)
    {
        _client = waf.Client;
    }

    /// <summary>
    /// Test if the health check endpoint is working
    /// </summary>
    [Fact]
    public async Task Should_return_200_ok_When_send_hc()
    {
        // Check if the default endpoint is working
        var response = await _client.PostAsJsonAsync("/hc", new { });

        Assert.True(response.IsSuccessStatusCode);
    }

    /// <summary>
    /// Test if the health check endpoint is working
    /// </summary>
    [Fact]
    public async Task Should_not_bad_When_send_hc()
    {
        // Check if the default endpoint is working
        var response = await _client.PostAsJsonAsync("/hc", new { });
        var isSuccess = !response.IsSuccessStatusCode;

        Assert.False(isSuccess);
    }
}
