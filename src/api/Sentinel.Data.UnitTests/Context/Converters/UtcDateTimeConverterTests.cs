using Sentinel.Data.Context.Converters;

namespace Sentinel.Data.UnitTests.Context.Converters;

public static class UtcDateTimeConverterTests
{
    private static readonly DateTime UtcInstant = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

    public class ToProvider
    {
        [Fact]
        public void ReturnsValueUnchanged_WhenUtc()
        {
            var actual = UtcDateTimeConverter.ToProvider(UtcInstant);

            Assert.Equal(UtcInstant, actual);
            Assert.Equal(DateTimeKind.Utc, actual.Kind);
        }

        [Fact]
        public void ConvertsToUtc_WhenLocal()
        {
            var actual = UtcDateTimeConverter.ToProvider(UtcInstant.ToLocalTime());

            Assert.Equal(UtcInstant, actual);
            Assert.Equal(DateTimeKind.Utc, actual.Kind);
        }

        [Fact]
        public void IsWiredIntoTheConverterExpression()
        {
            var actual = new UtcDateTimeConverter().ConvertToProvider(UtcInstant.ToLocalTime());

            Assert.Equal(UtcInstant, actual);
        }
    }

    public class FromProvider
    {
        [Fact]
        public void MarksValueAsUtc_WithoutShiftingIt()
        {
            var stored = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Unspecified);

            var actual = UtcDateTimeConverter.FromProvider(stored);

            Assert.Equal(UtcInstant, actual);
            Assert.Equal(DateTimeKind.Utc, actual.Kind);
        }

        [Fact]
        public void IsWiredIntoTheConverterExpression()
        {
            var stored = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Unspecified);

            var actual = (DateTime)new UtcDateTimeConverter().ConvertFromProvider(stored)!;

            Assert.Equal(DateTimeKind.Utc, actual.Kind);
        }
    }
}