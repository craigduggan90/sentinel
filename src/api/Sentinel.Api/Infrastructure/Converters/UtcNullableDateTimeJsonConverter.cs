using System.Text.Json;

namespace Sentinel.Api.Infrastructure.Converters;

public class UtcNullableDateTimeJsonConverter : UtcDateTimeJsonConverterBase<DateTime?>
{
    // System.Text.Json intercepts null itself for a Nullable<T> converter (HandleNull is false) and never calls Read
    // with a null token or Write with a null value - both methods rely on that guarantee rather than defending
    // against it. The null round-trip is covered by UtcNullableDateTimeJsonConverterNullTests.
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        EnsureUtcDateTime(reader.GetDateTime());

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options) =>
        writer.WriteStringValue(EnsureUtcDateTime(value!.Value));
}