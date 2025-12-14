using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
public class JobApprovalController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddReviewJob([FromForm] SaveJobReviewCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToActionResult();
    }

}
