using Beans.Patchable.Serialization;
using Sentinel.Api.Infrastructure.Converters;
using Sentinel.Api.Infrastructure.Errors;
using Sentinel.Api.Infrastructure.Swagger;
using Sentinel.Api.Infrastructure.Versioning;
using Sentinel.Api.Services;
using Sentinel.Common;
using Sentinel.Core;
using Sentinel.Core.Services;
using Sentinel.Data;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder ConfigureSentinelServices(this WebApplicationBuilder builder)
    {
        builder
            .AddCommonServices()
            .AddCoreServices()
            .AddDataServices()
            .AddVersioning()
            .AddSwaggerDocumentation()
            .AddErrorHandling();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IActorAccessor, ActorAccessor>();

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new UtcDateTimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new UtcNullableDateTimeJsonConverter());

                // Beans.Patchable: lets PATCH request models distinguish "not sent" from "sent as null".
                options.JsonSerializerOptions.Converters.Add(new OptionalJsonConverterFactory());
            });
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        return builder;
    }

    public static WebApplication ConfigureSentinelApplication(this WebApplication app)
    {
        app.UseSwaggerDocumentation();
        app.UseErrorHandling();

        app.UseRouting();
        app.MapControllers();
        return app;
    }
}