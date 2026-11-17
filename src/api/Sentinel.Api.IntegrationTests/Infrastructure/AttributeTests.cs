using Sentinel.Api.Infrastructure;
using Sentinel.Api.IntegrationTests.TestControllers;
using Sentinel.Core.Models;
using System.Net;

namespace Sentinel.Api.IntegrationTests.Infrastructure;

public class AttributeTests(ApiWebApplicationFactory factory) : ApiControllerTestsBase(factory)
{
    [Theory]
    [InlineData(null, HttpStatusCode.Forbidden)]
    [InlineData("something-else", HttpStatusCode.Forbidden)]
    [InlineData("something-else, " + Scopes.Authorizer, HttpStatusCode.NoContent)]
    public async Task RequiresScope_GatesOnTheScopesHeader(string? scopes, HttpStatusCode expectedStatus)
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/scoped", scopes: scopes), CancellationToken);

        Assert.Equal(expectedStatus, response.StatusCode);
    }

    [Fact]
    public async Task RequiresScope_MatchesTheScopeCaseInsensitively()
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/scoped", scopes: Scopes.Authorizer.ToUpperInvariant()),
            CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RequiresHeader_Returns428_WhenHeaderMissing()
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/header"), CancellationToken);

        Assert.Equal((HttpStatusCode)428, response.StatusCode);
        var problemDetails = await ReadProblemDetailsAsync(response);
        Assert.Equal("'Idempotency-Key' header value is required.", problemDetails!.Detail);
    }

    [Fact]
    public async Task RequiresHeader_PassesThrough_WhenHeaderPresent()
    {
        var request = CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/header");
        request.Headers.Add("Idempotency-Key", Guid.NewGuid().ToString());

        var response = await Client.SendAsync(request, CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task RequiresCurrentActor_Returns428_WhenActorHeadersMissing()
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/actor"), CancellationToken);

        Assert.Equal((HttpStatusCode)428, response.StatusCode);
        var problemDetails = await ReadProblemDetailsAsync(response);
        Assert.Equal("'Sentinel-User-Id' header value is required.", problemDetails!.Detail);
    }

    [Fact]
    public async Task RequiresCurrentActor_ResolvesActorFromHeaders()
    {
        var request = WithActorHeaders(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/actor"), "user-001", "Ada Lovelace");

        var response = await Client.SendAsync(request, CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new Actor("user-001", "Ada Lovelace"), await ReadContentAsync<Actor>(response));
    }
}