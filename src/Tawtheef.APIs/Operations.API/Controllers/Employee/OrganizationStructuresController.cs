
using Cortex.Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Common.Security.Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Commands;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/[controller]")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrganizationStructuresController(IMediator mediator) : ControllerBase
{
    [HttpGet("sectors")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> GetSectors([FromQuery] GetSectorsPagedQuery query, CancellationToken cancellationToken)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language }, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("managements")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> GetManagements([FromQuery] GetManagementsPagedQuery query, CancellationToken cancellationToken)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language }, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("departments")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> GetDepartments([FromQuery] GetDepartmentsPagedQuery query, CancellationToken cancellationToken)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language }, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("sectors")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> CreateSector([FromBody] CreateSectorCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("sectors/{id:guid}")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> UpdateSector(Guid id, [FromBody] UpdateSectorCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { Id = id }, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("sectors/change-activation")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> ChangeSectorActivation([FromBody] ChangeSectorActivationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("managements")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> CreateManagement([FromBody] CreateManagementCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("managements/{id:guid}")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> UpdateManagement(Guid id, [FromBody] UpdateManagementCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { Id = id }, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("managements/change-activation")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> ChangeManagementActivation([FromBody] ChangeManagementActivationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("departments")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("departments/{id:guid}")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> UpdateDepartment(Guid id, [FromBody] UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { Id = id }, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("departments/change-activation")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> ChangeDepartmentActivation([FromBody] ChangeDepartmentActivationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("lookups/sectors")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> GetSectorsLookup([FromQuery] GetSectorsQuery query)
    {
        var language = Request.Headers.AcceptLanguage.ToString();
        var result = await mediator.Send(query with { Language = language });
        return result.ToActionResult();
    }

    [HttpGet("lookups/managements")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> GetManagementLookup([FromQuery] Guid sectorId)
    {
        var result = await mediator.Send(new GetManagementsBySectorQuery(sectorId));
        return result.ToActionResult();
    }

    [HttpGet("lookups/departments")]
    [AuthorizePermission(PermissionKeys.OrganizationStructures.Manage)]
    public async Task<IActionResult> GetDepartmentLookup([FromQuery] Guid managementId)
    {
        var result = await mediator.Send(new GetDepartmentsByManagementQuery(managementId));
        return result.ToActionResult();
    }
}
