using Cortex.Mediator.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Tawtheef.Application.Features.Resources.Queries;

public sealed record GetPublicFileQuery(string Path) : IQuery<IActionResult>;
