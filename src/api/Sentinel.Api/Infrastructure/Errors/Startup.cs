using Sentinel.Api.Infrastructure.Errors.Handlers;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure.Errors;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        // Handlers run in registration order - UnhandledExceptionHandler must stay last.
        builder.Services
            .AddExceptionHandler<MissingHeaderExceptionHandler>()
            .AddExceptionHandler<MissingScopeExceptionHandler>()
            .AddExceptionHandler<NotFoundExceptionHandler>()
            .AddExceptionHandler<ValidationExceptionHandler>()
            .AddExceptionHandler<AccessDeniedExceptionHandler>()
            .AddExceptionHandler<UnhandledExceptionHandler>()
            .AddProblemDetails();

        return builder;
    }

    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
        => app.UseExceptionHandler();
}