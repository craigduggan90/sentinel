using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sentinel.Data.Context;

namespace Sentinel.Data.UnitTests.Context;

public static class ApiDbContextFactoryTests
{
    private const string ReadOnlyConnectionString = "Data Source=:memory:;Mode=ReadOnly";
    private const string ReadWriteConnectionString = "Data Source=:memory:";

    public class CreateDbContext
    {
        [Fact]
        public void CreatesReadOnlyContext_WhenNoParameterProvided()
        {
            using var context = CreateSut().CreateDbContext();

            Assert.Equal(ReadOnlyConnectionString, context.Database.GetConnectionString());
        }

        [Fact]
        public void CreatesReadOnlyContext_WhenContextTypeReadOnly()
        {
            using var context = CreateSut().CreateDbContext(ContextType.ReadOnly);

            Assert.Equal(ReadOnlyConnectionString, context.Database.GetConnectionString());
        }

        [Fact]
        public void CreatesReadWriteContext_WhenContextTypeReadWrite()
        {
            using var context = CreateSut().CreateDbContext(ContextType.ReadWrite);

            Assert.Equal(ReadWriteConnectionString, context.Database.GetConnectionString());
        }

        [Theory]
        [InlineData(ContextType.ReadOnly, "Reader")]
        [InlineData(ContextType.ReadWrite, "Writer")]
        public void Throws_WhenConnectionStringNotConfigured(ContextType contextType, string missingKey)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                [
                    new KeyValuePair<string, string?>("ConnectionStrings:Reader", missingKey == "Reader" ? null : ReadOnlyConnectionString),
                    new KeyValuePair<string, string?>("ConnectionStrings:Writer", missingKey == "Writer" ? null : ReadWriteConnectionString)
                ])
                .Build();
            var sut = new ApiDbContextFactory(configuration);

            var exception = Assert.Throws<ArgumentException>(() => sut.CreateDbContext(contextType));

            Assert.Equal("contextType", exception.ParamName);
            Assert.Contains(contextType.ToString(), exception.Message);
        }
    }

    private static ApiDbContextFactory CreateSut() => new(
        new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("ConnectionStrings:Reader", ReadOnlyConnectionString),
                new KeyValuePair<string, string?>("ConnectionStrings:Writer", ReadWriteConnectionString)
            ])
            .Build());
}