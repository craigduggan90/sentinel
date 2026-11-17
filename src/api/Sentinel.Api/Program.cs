using Sentinel.Api.Infrastructure;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args).ConfigureSentinelServices();
var app = builder.Build().ConfigureSentinelApplication();

app.Run();

namespace Sentinel.Api
{
    /// <summary>This partial class is required for testing and exclusion from test coverage metrics.</summary>
    [ExcludeFromCodeCoverage]
    public partial class Program;
}