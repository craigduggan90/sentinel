using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Sentinel.Api.Infrastructure.Errors.Handlers;
using static Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers.ExceptionHandlerTestHelpers;

namespace Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers;

public static class UnhandledExceptionHandlerTests
{
    public class TryHandleAsync
    {
        private readonly FakeLogger<UnhandledExceptionHandler> _logger = new();

        [Fact]
        public async Task WritesGenericProblemDetailsAndLogs_ForAnyException()
        {
            var exception = new InvalidOperationException("that was an invalid operation!");

            var (statusCode, content) = await HandleAsync(new UnhandledExceptionHandler(_logger), exception);

            Assert.Equal(500, statusCode);
            AssertCoreFieldsEqual(UnhandledExceptionHandler.GetProblemDetails(), content);

            var log = Assert.Single(_logger.Collector.GetSnapshot());
            Assert.Equal(LogLevel.Error, log.Level);
            Assert.Same(exception, log.Exception);
        }

        [Fact]
        public async Task DoesNotLeakExceptionDetail()
        {
            var (_, content) = await HandleAsync(
                new UnhandledExceptionHandler(_logger), new InvalidOperationException("secret internals"));

            Assert.DoesNotContain("secret internals", content.Detail);
            Assert.Equal("https://sentinel.api/errors/unhandled", content.Type);
        }
    }
}