using Beans.Common.Providers;
using Sentinel.Common.Providers;

namespace Sentinel.Common.UnitTests.Providers;

public static class IdentifierProviderTests
{
    public class Generate
    {
        [Fact]
        public void ReturnsGuidWithoutHyphens_WhenNoContextIsActive()
        {
            var guid = new Guid("7e0c1088-c1ef-4eff-8711-5638e9d99f4f");
            using var _ = new GuidProviderContext(guid);

            Assert.Equal("7e0c1088c1ef4eff87115638e9d99f4f", IdentifierProvider.Generate());
        }

        [Fact]
        public void ReturnsUniqueValues_WhenNoContextIsActive()
        {
            Assert.NotEqual(IdentifierProvider.Generate(), IdentifierProvider.Generate());
        }

        [Fact]
        public void ReturnsFixedValue_WhenContextIsActive()
        {
            using var _ = new IdentifierProviderContext("fixed-id");

            Assert.Equal("fixed-id", IdentifierProvider.Generate());
            Assert.Equal("fixed-id", IdentifierProvider.Generate());
        }

        [Fact]
        public void ReturnsInnermostValue_WhenContextsAreNested()
        {
            using var outer = new IdentifierProviderContext("outer-id");

            using (new IdentifierProviderContext("inner-id"))
                Assert.Equal("inner-id", IdentifierProvider.Generate());

            Assert.Equal("outer-id", IdentifierProvider.Generate());
        }

        [Fact]
        public void StopsReturningFixedValue_WhenContextIsDisposed()
        {
            using (new IdentifierProviderContext("fixed-id"))
                Assert.Equal("fixed-id", IdentifierProvider.Generate());

            Assert.NotEqual("fixed-id", IdentifierProvider.Generate());
        }
    }
}