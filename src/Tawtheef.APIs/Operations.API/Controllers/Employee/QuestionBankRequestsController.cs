using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using Application.Operation.Features.Employee.QuestionBankRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class QuestionBankRequestsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.QuestionBankRequests.View)]
    public async Task<IActionResult> List([FromQuery] ListQuestionBankRequestsQuery query, CancellationToken cancellationToken) =>
        (await mediator.Send(query, cancellationToken)).ToActionResult();

    [HttpPost]
    [AuthorizePermission(PermissionKeys.QuestionBankRequests.Create)]
    public async Task<IActionResult> Create([FromBody] CreateQuestionBankRequestCommand command, CancellationToken cancellationToken) =>
        (await mediator.Send(command, cancellationToken)).ToActionResult();

    [HttpGet("lookups/question-bank-types")]
    [AuthorizePermission(PermissionKeys.QuestionBankRequests.View)]
    public async Task<IActionResult> QuestionBankTypes(CancellationToken cancellationToken) =>
        (await mediator.Send(new GetQuestionBankTypesQuery { Language = Request.Headers.AcceptLanguage }, cancellationToken)).ToActionResult();

    [HttpGet("lookups/managements")]
    [AuthorizePermission(PermissionKeys.QuestionBankRequests.View)]
    public async Task<IActionResult> Managements(CancellationToken cancellationToken) =>
        (await mediator.Send(new GetManagemntsQuery { Language = Request.Headers.AcceptLanguage }, cancellationToken)).ToActionResult();

    [HttpGet("lookups/job-titles")]
    [AuthorizePermission(PermissionKeys.QuestionBankRequests.View)]
    public async Task<IActionResult> JobTitles(CancellationToken cancellationToken) =>
        (await mediator.Send(new GetJobTitlesQuery { Language = Request.Headers.AcceptLanguage, PaginatedRequest = new PaginatedRequest { PageNumber = 1, PageSize = 10000 } }, cancellationToken)).ToActionResult();

    [HttpGet("lookups/request-types")]
    [AuthorizePermission(PermissionKeys.QuestionBankRequests.View)]
    public async Task<IActionResult> RequestTypes(CancellationToken cancellationToken) =>
        (await mediator.Send(new GetQuestionBankRequestTypesQuery { Language = Request.Headers.AcceptLanguage }, cancellationToken)).ToActionResult();

    [HttpGet("lookups/statuses")]
    [AuthorizePermission(PermissionKeys.QuestionBankRequests.View)]
    public async Task<IActionResult> Statuses(CancellationToken cancellationToken) =>
        (await mediator.Send(new GetQuestionBankRequestStatusesQuery { Language = Request.Headers.AcceptLanguage }, cancellationToken)).ToActionResult();
}
