using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Services;

// "Which jobs' schedules may this caller browse via Start Interview" - a plain committee member
// (InterviewEvaluation permission only, no InterviewSchedule) must only see the schedules of jobs
// whose committee they actually sit on; Chair/HR/SuperAdmin (the existing HasSummaryRoleBypass rule)
// see everything, same bypass already used for the committee evaluation summary. Reused by
// ListMySessionsQuery/GetMySessionQuery/ListMySessionAppointmentsQuery so the three stay consistent.
public sealed class EvaluationSessionAccessResolver(IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver)
{
    // null = no restriction (bypass). Empty set = no accessible jobs at all.
    public async Task<HashSet<Guid>?> GetAccessibleJobIdsAsync(CancellationToken cancellationToken)
    {
        if (accessResolver.HasSummaryRoleBypass())
            return null;

        var currentUserId = accessResolver.GetCurrentUserId();
        if (currentUserId is null)
            return [];

        var jobIds = await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
            .AsNoTracking()
            .Where(m => m.MemberUserId == currentUserId.Value && m.IsActive)
            .Join(unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet.AsNoTracking(),
                m => m.InterviewCommitteeId, c => c.Id, (m, c) => c.JobId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return jobIds.ToHashSet();
    }

    public async Task<bool> CanAccessJobAsync(Guid jobId, CancellationToken cancellationToken)
    {
        var accessibleJobIds = await GetAccessibleJobIdsAsync(cancellationToken);
        return accessibleJobIds is null || accessibleJobIds.Contains(jobId);
    }
}
