using Sentinel.Api.IntegrationTests.TestControllers;
using System.Net;
using System.Text.Json;

namespace Sentinel.Api.IntegrationTests.Infrastructure;

public class DataWiringTests(ApiWebApplicationFactory factory) : ApiControllerTestsBase(factory)
{
    [Fact]
    public async Task ResolvesDataServicesAgainstTheTestDatabase()
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Post, $"{PipelineTestController.Route}/database"), CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = JsonDocument.Parse(await response.Content.ReadAsStringAsync(CancellationToken)).RootElement;
        Assert.True(body.GetProperty("canConnect").GetBoolean());
        Assert.True(body.GetProperty("sharesContext").GetBoolean());
        Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", body.GetProperty("provider").GetString());
    }
}