using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace LibraryManagement.Api.Features.Info;

[ApiController]
[Route("api/info")]
public class InfoController(IHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public ActionResult<InfoResponse> Get()
    {
        var assembly = typeof(Program).Assembly;

        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?? assembly.GetName().Version?.ToString()
            ?? "unknown";

        return new InfoResponse(
            assembly.GetName().Name ?? "LibraryManagement.Api",
            version,
            environment.EnvironmentName,
            DateTime.UtcNow
        );
    }
}
