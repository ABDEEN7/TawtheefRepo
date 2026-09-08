using Application.Operation.Features.Employee.QuestionBanks.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class QuestionBanksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.QuestionBanks.View)]
    public async Task<IActionResult> ListQuestionBanks(
        [FromQuery] ListQuestionBanksQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
