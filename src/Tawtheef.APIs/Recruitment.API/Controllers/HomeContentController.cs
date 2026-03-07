using Application.Recruitment.Features.HomeContent.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("api/home-content")]
public class HomeContentController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetHomeContent()
    {
        var result = await mediator.Send(new GetHomeContentQuery());
        return result.ToActionResult();
    }
}

