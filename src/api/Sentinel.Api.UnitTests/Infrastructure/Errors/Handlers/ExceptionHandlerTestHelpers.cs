using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Sentinel.Api.UnitTests.Infrastructure.Errors.Handlers;

internal static class ExceptionHandlerTestHelpers
{
    public static async Task AssertIgnoresAsync(IExceptionHandler sut)
    {
        var context = new DefaultHttpContext();

        var handled = await sut.TryHandleAsync(
            context, new InvalidOperationException("not mine"), TestContext.Current.CancellationToken);

        Assert.False(handled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    public static async Task<(int StatusCode, ProblemDetails Content)> HandleAsync(IExceptionHandler sut, Exception exception)
    {
        using var responseStream = new MemoryStream();
        var context = new DefaultHttpContext { Response = { Body = responseStream } };

        var handled = await sut.TryHandleAsync(context, exception, TestContext.Current.CancellationToken);

        Assert.True(handled);
        var content = await responseStream.RewindAndReadAsync<ProblemDetails>();
        Assert.NotNull(content);
        return (context.Response.StatusCode, content);
    }

    public static void AssertCoreFieldsEqual(ProblemDetails expected, ProblemDetails actual)
    {
        Assert.Equal(expected.Type, actual.Type);
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Detail, actual.Detail);
        Assert.Equal(expected.Status, actual.Status);
    }

    public static string? GetExtensionString(ProblemDetails problemDetails, string key) =>
        problemDetails.Extensions.TryGetValue(key, out var value) ? ((JsonElement)value!).GetString() : null;
}