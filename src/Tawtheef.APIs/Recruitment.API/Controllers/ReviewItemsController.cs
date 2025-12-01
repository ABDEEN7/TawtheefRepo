using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Infrastructure.Extensions;

namespace Recruitment.API.Controllers;

[ApiController]
[Route("api/review-items")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ReviewItemsController(IMediator mediator) : ControllerBase
{
    private Result<Guid> UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value switch
    {
        null => Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier),
        var id => Result.Ok(Guid.Parse(id))
    };

    [HttpPatch("{reviewItemId}")]
    public async Task<IActionResult> UpdateStatus(Guid reviewItemId, [FromBody] UpdateReviewItemStatusRequest request, CancellationToken ct)
    {
        if (UserId.IsFailed) return BadRequest(UserId.Errors);

        var cmd = new ReviewProfileItemCommand(UserId.Value, reviewItemId, request.Status, request.Note);
        var result = await mediator.Send(cmd, ct);
        return result.ToActionResult();
    }
}

public sealed class UpdateReviewItemStatusRequest
{
    public ReviewStatus Status { get; set; }
    public string? Note { get; set; }
}
