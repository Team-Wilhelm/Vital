using System.Net.Http.Json;
using IntegrationTests.ApiFactor;

namespace IntegrationTests;

public class HealthCheckTests
{
    /// <summary>
    /// Test if the health check endpoint is working
    /// </summary>
    [Test]
    public async Task Should_return_200_ok_When_send_hc()
    {
        var api = new VitalApiFactory();
        var httpClient = api.CreateClient();

        // Check if the default endpoint is working
        var response = await httpClient.PostAsJsonAsync("/hc", new { });

        Assert.True(response.IsSuccessStatusCode);
    }

    /// <summary>
    /// Test if the health check endpoint is working
    /// </summary>
    [Test]
    public async Task Should_not_bad_When_send_hc()
    {
        var api = new VitalApiFactory();
        var httpClient = api.CreateClient();

        // Check if the default endpoint is working
        var response = await httpClient.PostAsJsonAsync("/hc", new { });
        var isSuccess = !response.IsSuccessStatusCode;

        Assert.IsFalse(isSuccess);
    }
}
