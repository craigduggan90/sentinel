using Sentinel.Api.IntegrationTests.TestControllers;
using System.Net;
using System.Text.Json;

namespace Sentinel.Api.IntegrationTests.Infrastructure;

public class ErrorHandlingTests(ApiWebApplicationFactory factory) : ApiControllerTestsBase(factory)
{
    [Theory]
    [InlineData("access-denied", HttpStatusCode.Forbidden, "forbidden")]
    [InlineData("not-found", HttpStatusCode.NotFound, "not-found")]
    [InlineData("command-validation", HttpStatusCode.UnprocessableEntity, "validation")]
    [InlineData("query-validation", HttpStatusCode.BadRequest, "validation")]
    [InlineData("missing-header", (HttpStatusCode)428, "missing-header")]
    [InlineData("missing-scope", HttpStatusCode.Forbidden, "missing-scope")]
    [InlineData("anything-else", HttpStatusCode.InternalServerError, "unhandled")]
    public async Task MapsEachExceptionToItsProblemDetails(string kind, HttpStatusCode expectedStatus, string expectedType)
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/throw/{kind}"), CancellationToken);

        Assert.Equal(expectedStatus, response.StatusCode);
        var problemDetails = await ReadProblemDetailsAsync(response);
        Assert.NotNull(problemDetails);
        Assert.Equal((int)expectedStatus, problemDetails.Status);
        Assert.Equal($"https://sentinel.api/errors/{expectedType}", problemDetails.Type);
    }

    [Fact]
    public async Task IncludesResourceDetails_ForNotFound()
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/throw/not-found"), CancellationToken);

        var problemDetails = await ReadProblemDetailsAsync(response);
        Assert.Equal("Widget", ((JsonElement)problemDetails!.Extensions["resource"]!).GetString());
        Assert.Equal("widget-001", ((JsonElement)problemDetails.Extensions["identifier"]!).GetString());
    }

    [Fact]
    public async Task DoesNotLeakExceptionMessages_ForUnhandledExceptions()
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/throw/anything-else"), CancellationToken);

        Assert.DoesNotContain("secret", await response.Content.ReadAsStringAsync(CancellationToken));
    }

    [Fact]
    public async Task ReturnsValidationErrorsByProperty_ForAFailingValidator()
    {
        var response = await Client.SendAsync(
            CreateJsonRequest(HttpMethod.Post, $"{PipelineTestController.Route}/validate", new { Name = "far too long" }),
            CancellationToken);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var problemDetails = await ReadProblemDetailsAsync(response);
        Assert.Single(GetValidationErrors(problemDetails!, "Name"));
    }

    [Fact]
    public async Task ReturnsNoContent_ForAPassingValidator()
    {
        var response = await Client.SendAsync(
            CreateJsonRequest(HttpMethod.Post, $"{PipelineTestController.Route}/validate", new { Name = "ok" }),
            CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ReturnsUnhandledProblemDetails_WhenTheMediatorHasNoHandler()
    {
        var response = await Client.SendAsync(
            CreateRequest(HttpMethod.Get, $"{PipelineTestController.Route}/mediator"), CancellationToken);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var problemDetails = await ReadProblemDetailsAsync(response);
        Assert.Equal("https://sentinel.api/errors/unhandled", problemDetails!.Type);
    }
}