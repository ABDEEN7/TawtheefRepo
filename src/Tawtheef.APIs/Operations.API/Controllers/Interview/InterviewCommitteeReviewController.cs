using Application.Operation.Features.Employee.Interview.CommitteeReview.Commands;
using Application.Operation.Features.Employee.Interview.CommitteeReview.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Tawtheef.Application.Common.Security;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Infrastructure.Extensions;

namespace Operations.API.Controllers.Interview;

// Committee Head Review - the chair's step between result generation and Approve Interview Results.
// The View permission opens the endpoints; the handlers restrict access to the schedule's own chair
// (or the HR bypass roles), and SaveCommitteeReview checks OverrideSuggestion for decision overrides.
[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class InterviewCommitteeReviewController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [AuthorizePermission(PermissionKeys.InterviewCommitteeReview.View)]
    public async Task<IActionResult> ListCommitteeReviews(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListCommitteeReviewsQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("schedule/{scheduleId:guid}")]
    [AuthorizePermission(PermissionKeys.InterviewCommitteeReview.View)]
    public async Task<IActionResult> GetCommitteeReviewBySchedule(Guid scheduleId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCommitteeReviewByScheduleQuery(scheduleId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut]
    [AuthorizePermission(PermissionKeys.InterviewCommitteeReview.View)]
    public async Task<IActionResult> SaveCommitteeReview([FromBody] SaveCommitteeReviewCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("lookups/school-stages")]
    [AuthorizePermission(PermissionKeys.InterviewCommitteeReview.View)]
    public async Task<IActionResult> GetSchoolStages([FromQuery] GetSchoolStagesQuery query, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return result.ToActionResult();
    }
}
