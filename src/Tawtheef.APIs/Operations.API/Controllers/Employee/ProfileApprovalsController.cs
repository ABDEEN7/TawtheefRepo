using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Commands;
using Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.Queries;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Infrastructure.Extensions;
using Tawtheef.Infrastructure.Services.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/profile-approvals")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//[Authorize(Policy = PermissionPolicyProvider.POLICY_PREFIX + PermissionNames.JobsManage)]
public class ProfileApprovalsController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };

    private bool HasManagerOverride => User.IsInRole("DepartmentHead") || User.IsInRole("Manager");

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetProfileApprovalsQuery query, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var enriched = new GetProfileApprovalsQuery(
            UserId.Value,
            query.Search,
            query.Specialization,
            query.Status,
            query.TargetEntity,
            query.CandidateType,
            query.Sort,
            query.SortDirection);

        var result = await mediator.Send(enriched, ct);
        return result.ToActionResult();
    }

    [HttpGet("{userProfileId:guid}")]
    public async Task<IActionResult> GetDetail(Guid userProfileId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetProfileApprovalDetailQuery(userProfileId, UserId.Value), ct);
        return result.ToActionResult();
    }

    [HttpGet("{userProfileId:guid}/changes")]
    public async Task<IActionResult> GetPartialChanges(Guid userProfileId, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var result = await mediator.Send(new GetProfilePartialChangesQuery(userProfileId, UserId.Value), ct);
        return result.ToActionResult();
    }

    [HttpPatch("review-items/{reviewItemId:guid}")]
    public async Task<IActionResult> UpdateReviewItem(Guid reviewItemId, [FromBody] UpdateReviewItemStatusRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var cmd = new ReviewProfileItemCommand(UserId.Value, reviewItemId, request.Status, request.Note);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPost("{userProfileId:guid}/finalize")]
    public async Task<IActionResult> FinalizeProfile(Guid userProfileId, [FromForm] FinalizeProfileApprovalRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var cmd = new FinalizeProfileApprovalCommand(
            UserId.Value,
            userProfileId,
            request.Action,
            request.Notes,
            request.Summary,
            request.NeedsCorrectionItems ?? Array.Empty<Guid>(),
            request.RejectionDocument,
            request.ExceptionalFile,
            HasManagerOverride);

        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
}

public sealed class UpdateReviewItemStatusRequest
{
    public ReviewStatus Status { get; set; }
    public string? Note { get; set; }
}

public sealed class FinalizeProfileApprovalRequest
{
    public FinalApprovalAction Action { get; set; }
    public string? Notes { get; set; }
    public string? Summary { get; set; }
    public IReadOnlyCollection<Guid>? NeedsCorrectionItems { get; set; }
    public IFormFile? RejectionDocument { get; set; }
    public IFormFile? ExceptionalFile { get; set; }
}
