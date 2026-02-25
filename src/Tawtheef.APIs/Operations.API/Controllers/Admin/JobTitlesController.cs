using Application.Operation.Features.Admin.JobTitles.Commands;
using Application.Operation.Features.Admin.JobTitles.Queries;
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Admin;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class JobTitlesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.Jobs.View, PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> List()
    {
        var result = await mediator.Send(new GetListJobTitlesQuery());
        return result.ToActionResult();
    }

    [HttpPost]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> Create([FromBody] UpsertJobTitleRequest request)
    {
        var result = await mediator.Send(new InsertJobTitleCommand(
            request.JobNumber,
            request.JobNameAr,
            request.JobNameEn));

        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertJobTitleRequest request)
    {
        var result = await mediator.Send(new UpdateJobTitleCommand(
            id,
            request.JobNumber,
            request.JobNameAr,
            request.JobNameEn));

        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    [AuthorizePermission(PermissionKeys.Jobs.Manage)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteJobTitleCommand(id));
        return result.ToActionResult();
    }

    public sealed class UpsertJobTitleRequest
    {
        public required string JobNumber { get; set; }
        public required string JobNameAr { get; set; }
        public required string JobNameEn { get; set; }
    }
}
