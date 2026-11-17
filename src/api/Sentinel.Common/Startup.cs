using Beans.Common;
using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Common;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static WebApplicationBuilder AddCommonServices(this WebApplicationBuilder builder)
    {
        // Beans.Common: IEnvironmentVariableAccessor (the providers are static and need no registration).
        builder.Services.AddCommonServices();
        return builder;
    }
}