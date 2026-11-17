using Beans.Requestable;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Core;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddCoreServices(this WebApplicationBuilder builder)
    {
        // Every IRequestHandler<,> and AbstractValidator<> under UseCases/ is picked up from this assembly.
        builder.Services.AddRequestableServices(options => options.Assemblies = [typeof(Startup).Assembly]);
        builder.Services.AddValidatorsFromAssembly(typeof(Startup).Assembly,
            includeInternalTypes: false,
            lifetime: ServiceLifetime.Singleton);

        return builder;
    }
}