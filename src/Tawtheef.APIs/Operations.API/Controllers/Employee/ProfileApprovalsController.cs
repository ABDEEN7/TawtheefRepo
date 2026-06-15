using System.Security.Claims;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Employee;

[ApiController]
[Route("api/profile-approvals")]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileApprovalsController(IMediator mediator) : ControllerBase
{
    private Result<Guid> OfficerId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };

    [HttpGet]
    [AuthorizePermission(PermissionKeys.ProfileApproval.View)]
    public async Task<IActionResult> GetList([FromQuery] GetProfileApprovalsQuery query, CancellationToken ct)
    {
        if (OfficerId.IsFailed) return BadRequest(OfficerId.Errors);

        var result = await mediator.Send(query with { OfficerId = OfficerId.Value }, ct);
        return result.ToActionResult();
    }

    [HttpGet("{userProfileId:guid}")]
    [AuthorizePermission(PermissionKeys.ProfileApproval.View)]
    public async Task<IActionResult> GetDetail(Guid userProfileId, CancellationToken ct = default)
    {
        if (OfficerId.IsFailed) return BadRequest(OfficerId.Errors);

        var result = await mediator.Send(new GetProfileApprovalDetailQuery(userProfileId, OfficerId.Value), ct);
        return result.ToActionResult();
    }

    
    [HttpGet("target-entities")]
    [AuthorizePermission(PermissionKeys.ProfileApproval.View)]
    public async Task<IActionResult> GetTargetEntities()
    {
        var result = await mediator.Send(new GetTargetEntitiesQuery());
        return result.ToActionResult();
    }
    
    [HttpGet("{userProfileId:guid}/changes")]
    [AuthorizePermission(PermissionKeys.ProfileApproval.Changes)]
    public async Task<IActionResult> GetChangesDetail(Guid userProfileId, CancellationToken ct = default)
    {
        if (OfficerId.IsFailed) return BadRequest(OfficerId.Errors);

        var result = await mediator.Send(new GetProfilePartialChangesQuery(userProfileId, OfficerId.Value), ct);
        return result.ToActionResult();
    }

    [HttpPost("{userProfileId:guid}/start-review")]
    [AuthorizePermission(PermissionKeys.ProfileApproval.Review)]
    public async Task<IActionResult> StartReview(Guid userProfileId, CancellationToken ct)
    {
        if (OfficerId.IsFailed) return BadRequest(OfficerId.Errors);

        var result = await mediator.Send(new StartUserProfileReviewCommand(OfficerId.Value, userProfileId), ct);
        return result.ToActionResult();
    }



    /// <summary>
    /// Sets decision per section during Full Review.
    /// This does NOT expose notes to the user until Finalize is done.
    /// </summary>
    [HttpPut("{userProfileId:guid}/sections/{section}/decision")]
    [AuthorizePermission(PermissionKeys.ProfileApproval.Review)]
    public async Task<IActionResult> DecideSection(
        Guid userProfileId,
        ProfileSection section,
        [FromBody] DecideProfileSectionRequest body,
        CancellationToken ct)
    {
        if (OfficerId.IsFailed) return BadRequest(OfficerId.Errors);

        // ظپظٹ ظ…ط±ط­ظ„ط© Full Review ظ†ط³ظ…ط­ Approved ط£ظˆ NeedsCorrection ظپظ‚ط·
        if (body.Status == ReviewStatus.NeedsCorrection && string.IsNullOrWhiteSpace(body.Note))
            return BadRequest(Result.Fail(ErrorsCodes.NotesRequiredForCorrection).Errors);

        var cmd = new DecideProfileSectionCommand(
            OfficerId: OfficerId.Value,
            UserProfileId: userProfileId,
            Section: section,
            Status: body.Status,
            Note: body.Note);

        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    [HttpPut("review-items/{reviewItemId:guid}")]
    [AuthorizePermission(PermissionKeys.ProfileApproval.Review)]
    public async Task<IActionResult> DecideReviewItem(
        Guid reviewItemId,
        [FromBody] DecideProfileReviewItemRequest body,
        CancellationToken ct)
    {
        if (OfficerId.IsFailed) return BadRequest(OfficerId.Errors);

        if (body.Status is ReviewStatus.Rejected or ReviewStatus.NeedsCorrection &&
            string.IsNullOrWhiteSpace(body.Note))
            return BadRequest(Result.Fail(ErrorsCodes.NotesRequiredForCorrection).Errors);

        var cmd = new DecideProfileReviewItemCommand(
            OfficerId: OfficerId.Value,
            ReviewItemId: reviewItemId,
            Status: body.Status,
            Note: body.Note,
            SpecializationRelation: body.SpecializationRelation);

        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Final decision gate:
    /// - If all sections Approved => Profile Approved
    /// - If any NeedsCorrection => Profile back to InCreation (then user sees notes)
    /// - If any Pending => fail
    /// </summary>
    [HttpPost("{userProfileId:guid}/finalize")]
    [AuthorizePermission(PermissionKeys.ProfileApproval.Review)]
    public async Task<IActionResult> Finalize(Guid userProfileId, [FromForm] FinalizeUserProfileReviewRequest body, CancellationToken ct)
    {
        if (OfficerId.IsFailed) return BadRequest(OfficerId.Errors);

        var cmd = new FinalizeUserProfileReviewCommand(
            OfficerId: OfficerId.Value,
            UserProfileId: userProfileId,
            Notes: body.Notes,
            Summary: body.Summary);

        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
}

