using Microsoft.AspNetCore.Mvc;
using Sentinel.Api.Controllers.V1.Abstract;

namespace Sentinel.Api.Controllers.V1;

public class HealthController : ApiControllerBase
{
    [HttpGet, ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Ping()
        => NoContent();
}