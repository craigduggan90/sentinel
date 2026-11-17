using System.Text.Json;

namespace Sentinel.Api.Infrastructure.Converters;

public class UtcDateTimeJsonConverter : UtcDateTimeJsonConverterBase<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        EnsureUtcDateTime(reader.GetDateTime());

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(EnsureUtcDateTime(value));
}