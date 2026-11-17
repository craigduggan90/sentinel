using System.Net;

namespace Sentinel.Api.IntegrationTests.Controllers.V1;

public static class HealthControllerTests
{
    private const string BaseUrl = "api/health";
    private const string VersionedUrl = "api/v1/Health";

    public class Ping(ApiWebApplicationFactory factory) : ApiControllerTestsBase(factory)
    {
        [Fact]
        public async Task ReturnsNoContent_WhenCalledWithVersionHeader()
        {
            var response = await Client.SendAsync(CreateRequest(HttpMethod.Get, BaseUrl, apiVersion: "1.0"), CancellationToken);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task ReturnsNoContent_WhenCalledWithVersionedUrl()
        {
            var response = await Client.SendAsync(CreateRequest(HttpMethod.Get, VersionedUrl), CancellationToken);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task ReturnsBadRequest_WhenCalledWithConflictingVersions()
        {
            var response = await Client.SendAsync(CreateRequest(HttpMethod.Get, VersionedUrl, apiVersion: "0.1"), CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ReturnsBadRequest_WhenCalledWithoutAnyVersion()
        {
            var response = await Client.SendAsync(CreateRequest(HttpMethod.Get, BaseUrl), CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}