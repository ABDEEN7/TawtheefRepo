using Microsoft.AspNetCore.Hosting;
using Tawtheef.Application.Common.Interfaces.Services;

namespace Tawtheef.Infrastructure.Services.Environment;

public class EnvironmentNameProvider(IWebHostEnvironment env) : IEnvironmentNameProvider
{
    public string EnvironmentName { get; } = env.EnvironmentName;
}
