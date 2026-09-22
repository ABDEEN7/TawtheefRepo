using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Interview.Schedule.Services;

public sealed record EligibleCandidate(Guid InvitationId, Guid? GenderId);

// Shared precondition/pool resolution (schedule.md "Preconditions before an InterviewSchedule can
// be created") used by Create, Update (resubmit) and the preview/context queries alike, so all
// four always agree on who is eligible and whether the committee/template are ready.
public static class ScheduleEligibilityResolver
{
    private static readonly AppointmentStatus[] TiedUpStatuses =
    [
        AppointmentStatus.Scheduled, AppointmentStatus.InInterview, AppointmentStatus.UnderEvaluation
    ];

    public static async Task<Result<InterviewCommittee>> GetApprovedCommitteeAsync(
        IUnitOfWork unitOfWork, Guid jobId, CancellationToken cancellationToken)
    {
        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .FirstOrDefaultAsync(c => c.JobId == jobId && c.IsActive, cancellationToken);
        if (committee is null)
            return Result.Fail<InterviewCommittee>(new Error(ErrorsCodes.InterviewScheduleCommitteeNotFound));

        if (committee.Status != CommitteeStatus.Approved)
            return Result.Fail<InterviewCommittee>(new Error(ErrorsCodes.InterviewScheduleCommitteeNotApproved));

        return Result.Ok(committee);
    }

    public static async Task<Result> EnsureTemplateHasApprovedVersionAsync(
        IUnitOfWork unitOfWork, Guid interviewTemplateId, CancellationToken cancellationToken)
    {
        var hasApprovedVersion = await unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet
            .AnyAsync(v => v.InterviewTemplateId == interviewTemplateId && v.Status == TemplateVersionStatus.Approved, cancellationToken);

        return hasApprovedVersion ? Result.Ok() : Result.Fail(new Error(ErrorsCodes.InterviewTemplateHasNoApprovedVersion));
    }

    // The "remaining candidates" pool: InterviewEligible invitations for the job that aren't
    // already tied up in another live appointment (Scheduled/InInterview/UnderEvaluation) - a
    // Completed one is done and no longer competes for a slot, and Held has no candidate at all.
    // excludeScheduleId lets a resubmit/preview ignore the very schedule being replaced (its old
    // appointments haven't been deleted from the DB yet at the point this runs).
    public static async Task<List<EligibleCandidate>> GetEligibleCandidatesAsync(
        IUnitOfWork unitOfWork, Guid jobId, CancellationToken cancellationToken, Guid? excludeScheduleId = null)
    {
        return await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .Where(i => i.JobId == jobId && i.InvitationStatusId == InvitationStatusIds.InterviewEligible)
            .Where(i => !unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
                .Any(a => a.InvitationId == i.Id && TiedUpStatuses.Contains(a.Status)
                    && (excludeScheduleId == null || a.InterviewScheduleId != excludeScheduleId)))
            .OrderBy(i => i.CreatedDate).ThenBy(i => i.Id)
            .Select(i => new EligibleCandidate(i.Id, i.Applicant!.Profile!.GenderId))
            .ToListAsync(cancellationToken);
    }
}
