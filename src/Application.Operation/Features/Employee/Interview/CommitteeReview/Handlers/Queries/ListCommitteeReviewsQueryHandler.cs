using Application.Operation.Features.Employee.Interview.CommitteeReview.DTOs;
using Application.Operation.Features.Employee.Interview.CommitteeReview.Queries;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.CommitteeReview.Handlers.Queries;

// The live result reports whose committee the caller chairs (all of them for the HR bypass roles).
// A schedule without a row here has no report yet - not every candidate is done.
public sealed class ListCommitteeReviewsQueryHandler(IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver)
    : IRequestHandler<ListCommitteeReviewsQuery, IResult<List<CommitteeReviewListItemDto>>>
{
    public async Task<IResult<List<CommitteeReviewListItemDto>>> Handle(ListCommitteeReviewsQuery request, CancellationToken cancellationToken)
    {
        // One committee per schedule, so any candidate's appointment names the report's committee.
        var reports = await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
            .AsNoTracking()
            .Select(r => new
            {
                r.Id,
                r.Code,
                r.InterviewScheduleId,
                r.Status,
                CommitteeId = r.Candidates.Select(c => c.InterviewAppointment!.InterviewCommitteeId).FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        if (!accessResolver.HasSummaryRoleBypass())
        {
            var currentUserId = accessResolver.GetCurrentUserId();
            var chairedCommitteeIds = currentUserId is null
                ? []
                : (await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
                    .AsNoTracking()
                    .Where(m => m.MemberUserId == currentUserId.Value && m.IsActive && m.Role == CommitteeRole.Chair)
                    .Select(m => m.InterviewCommitteeId)
                    .ToListAsync(cancellationToken))
                    .ToHashSet();

            reports = reports.Where(r => chairedCommitteeIds.Contains(r.CommitteeId)).ToList();
        }

        return Result.Ok(reports
            .Select(r => new CommitteeReviewListItemDto(r.InterviewScheduleId, r.Id, r.Code, r.Status))
            .ToList());
    }
}
