using Sentinel.Api.Infrastructure.Converters;
using System.Text.Json;

namespace Sentinel.Api.UnitTests.Infrastructure.Converters;

public static class UtcNullableDateTimeJsonConverterTests
{
    private sealed record TestModel(DateTime? Value);

    private static readonly DateTime UtcInstant = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

    private static JsonSerializerOptions CreateOptions() => new() { Converters = { new UtcNullableDateTimeJsonConverter() } };

    public class Read
    {
        [Theory]
        [InlineData("2026-01-01T10:00:00Z")]
        [InlineData("2026-01-01T12:00:00+02:00")]
        [InlineData("2026-01-01T10:00:00")]
        public void ReturnsUtcValue_RegardlessOfSuppliedOffset(string value)
        {
            var result = JsonSerializer.Deserialize<TestModel>($$"""{"Value":"{{value}}"}""", CreateOptions());

            Assert.NotNull(result);
            Assert.Equal(UtcInstant, result.Value);
            Assert.Equal(DateTimeKind.Utc, ((DateTime)result.Value!).Kind);
        }
    }

    public class Write
    {
        public static TheoryData<DateTime> Values => new()
        {
            UtcInstant,
            UtcInstant.ToLocalTime(),
            DateTime.SpecifyKind(UtcInstant, DateTimeKind.Unspecified)
        };

        [Theory]
        [MemberData(nameof(Values))]
        public void WritesUtcValue_RegardlessOfKind(DateTime value)
        {
            var json = JsonSerializer.Serialize(new TestModel(value), CreateOptions());

            Assert.Equal("""{"Value":"2026-01-01T10:00:00Z"}""", json);
        }
    }
}

public class UtcNullableDateTimeJsonConverterNullTests
{
    private sealed record TestModel(DateTime? Value);

    private static JsonSerializerOptions CreateOptions() => new() { Converters = { new UtcNullableDateTimeJsonConverter() } };

    [Fact]
    public void ReadsNull()
    {
        var result = JsonSerializer.Deserialize<TestModel>("""{"Value":null}""", CreateOptions());

        Assert.NotNull(result);
        Assert.Null(result.Value);
    }

    [Fact]
    public void WritesNull()
    {
        Assert.Equal("""{"Value":null}""", JsonSerializer.Serialize(new TestModel(null), CreateOptions()));
    }
}