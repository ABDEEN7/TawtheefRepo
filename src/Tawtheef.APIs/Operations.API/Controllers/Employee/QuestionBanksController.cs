using Application.Operation.Features.Employee.QuestionBanks.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class QuestionBanksController(IMediator mediator) : ControllerBase
{
    [HttpGet("lookups/question-bank-types")]
    [AuthorizePermission(PermissionKeys.QuestionBanks.View)]
    public async Task<IActionResult> GetQuestionBankTypes(CancellationToken cancellationToken)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetQuestionBankTypesQuery { Language = language }, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("lookups/managements")]
    [AuthorizePermission(PermissionKeys.QuestionBanks.View)]
    public async Task<IActionResult> GetManagements(CancellationToken cancellationToken)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetManagemntsQuery { Language = language }, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("lookups/job-titles")]
    [AuthorizePermission(PermissionKeys.QuestionBanks.View)]
    public async Task<IActionResult> GetJobTitles(CancellationToken cancellationToken)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var query = new GetJobTitlesQuery
        {
            Language = language,
            PaginatedRequest = new PaginatedRequest { PageNumber = 1, PageSize = 10000 }
        };
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("lookups/stages")]
    [AuthorizePermission(PermissionKeys.QuestionBanks.View)]
    public async Task<IActionResult> GetStages(CancellationToken cancellationToken)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(new GetStagesQuery { Language = language }, cancellationToken);
        return result.ToActionResult();
    }

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
