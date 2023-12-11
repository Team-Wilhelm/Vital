using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Infrastructure.Data;
using IntegrationTests.Setup;
using Models.Dto.Cycle;

namespace IntegrationTests.Tests;

[Collection("VitalApi")]
public class CycleTests(VitalApiFactory vaf) : TestBase(vaf)
{
    [Fact]
    public async Task Get_Should_be_unauthorized()
    {
        await ClearToken();

        var response = await _client.GetAsync("/Cycle");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_by_id_Should_be_unauthorized()
    {
        await ClearToken();

        var id = Guid.NewGuid();
        var response = await _client.GetAsync($"/Cycle/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_Should_be_unauthorized()
    {
        await ClearToken();

        var createCycleDto = new CreateCycleDto()
        {
            StartDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(10),
        };
        var response = await _client.PostAsync($"/Cycle", JsonContent.Create(createCycleDto));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_Should_be_unauthorized()
    {
        await ClearToken();

        var id = Guid.NewGuid();
        var updateCycleDto = new UpdateCycleDto()
        {
            StartDate = DateTimeOffset.Now,
            EndDate = DateTimeOffset.Now.AddDays(10),
        };
        var response = await _client.PutAsync($"/Cycle/{id}", JsonContent.Create(updateCycleDto));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
