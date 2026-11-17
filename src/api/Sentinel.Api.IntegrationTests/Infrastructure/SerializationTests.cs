using Sentinel.Api.IntegrationTests.TestControllers;
using System.Net;
using System.Text.Json;

namespace Sentinel.Api.IntegrationTests.Infrastructure;

public class SerializationTests(ApiWebApplicationFactory factory) : ApiControllerTestsBase(factory)
{
    private async Task<JsonElement> EchoAsync(string json)
    {
        var response = await Client.SendAsync(
            CreateRawJsonRequest(HttpMethod.Patch, $"{PipelineTestController.Route}/echo", json), CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return JsonDocument.Parse(await response.Content.ReadAsStringAsync(CancellationToken)).RootElement;
    }

    [Fact]
    public async Task DistinguishesUnsetOptionalFromNull()
    {
        var unset = await EchoAsync("""{"When":"2026-01-01T10:00:00Z"}""");
        var setToNull = await EchoAsync("""{"Name":null,"When":"2026-01-01T10:00:00Z"}""");
        var setToValue = await EchoAsync("""{"Name":"Ada","When":"2026-01-01T10:00:00Z"}""");

        Assert.False(unset.GetProperty("nameIsSet").GetBoolean());
        Assert.True(setToNull.GetProperty("nameIsSet").GetBoolean());
        Assert.Equal(JsonValueKind.Null, setToNull.GetProperty("name").ValueKind);
        Assert.True(setToValue.GetProperty("nameIsSet").GetBoolean());
        Assert.Equal("Ada", setToValue.GetProperty("name").GetString());
    }

    [Fact]
    public async Task NormalisesDateTimesToUtc_InBothDirections()
    {
        var echoed = await EchoAsync("""{"When":"2026-01-01T12:00:00+02:00","MaybeWhen":"2026-01-01T10:00:00"}""");

        Assert.Equal("2026-01-01T10:00:00Z", echoed.GetProperty("when").GetString());
        Assert.Equal("2026-01-01T10:00:00Z", echoed.GetProperty("maybeWhen").GetString());
    }

    [Fact]
    public async Task WritesNullableDateTimeAsNull_WhenNotSupplied()
    {
        var echoed = await EchoAsync("""{"When":"2026-01-01T10:00:00Z"}""");

        Assert.Equal(JsonValueKind.Null, echoed.GetProperty("maybeWhen").ValueKind);
    }
}