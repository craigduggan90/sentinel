using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Sentinel.Api.Infrastructure.Swagger;

[ExcludeFromCodeCoverage]
public class VersionedSwaggerOptions(IApiVersionDescriptionProvider provider)
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "Sentinel API",
                Version = description.ApiVersion.ToString(),
                Description = "Privacy-preserving age verification."
                              + (description.IsDeprecated ? " This version is deprecated." : string.Empty)
            });
        }
    }

    public void Configure(string? name, SwaggerGenOptions options) => Configure(options);
}