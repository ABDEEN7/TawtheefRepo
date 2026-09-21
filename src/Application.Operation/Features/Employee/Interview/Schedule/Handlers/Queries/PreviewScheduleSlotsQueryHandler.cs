using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Queries;

// Read-only dry run of the exact same rules Create/Update enforce for real, so the wizard's step
// 2/3 can show live capacity and candidate pairing before the final atomic submit.
public sealed class PreviewScheduleSlotsQueryHandler(IUnitOfWork unitOfWork, UserManager<User> userManager)
    : IRequestHandler<PreviewScheduleSlotsQuery, IResult<SchedulePlanPreviewDto>>
{
    public async Task<IResult<SchedulePlanPreviewDto>> Handle(PreviewScheduleSlotsQuery request, CancellationToken cancellationToken)
    {
        var committeeResult = await ScheduleEligibilityResolver.GetApprovedCommitteeAsync(unitOfWork, request.JobId, cancellationToken);
        if (committeeResult.IsFailed)
            return Result.Fail<SchedulePlanPreviewDto>(committeeResult.Errors);
        var committee = committeeResult.Value;

        var templateResult = await ScheduleEligibilityResolver.EnsureTemplateHasApprovedVersionAsync(
            unitOfWork, committee.InterviewTemplateId, cancellationToken);
        if (templateResult.IsFailed)
            return Result.Fail<SchedulePlanPreviewDto>(templateResult.Errors);

        var eligibleCandidates = await ScheduleEligibilityResolver.GetEligibleCandidatesAsync(
            unitOfWork, request.JobId, cancellationToken, excludeScheduleId: request.ExcludeScheduleId);

        var slotsResult = ScheduleAppointmentPlanner.GenerateSlots(
            request.Periods, request.InterviewType, request.DurationMinutes, request.BufferMinutes);
        if (slotsResult.IsFailed)
            return Result.Fail<SchedulePlanPreviewDto>(slotsResult.Errors);

        var conflictResult = await ScheduleConflictChecker.ValidateNoConflictsAsync(
            unitOfWork, committee.Id, slotsResult.Value, cancellationToken, excludeScheduleId: request.ExcludeScheduleId);
        if (conflictResult.IsFailed)
            return Result.Fail<SchedulePlanPreviewDto>(conflictResult.Errors);

        var eligibleIds = eligibleCandidates.Select(c => c.InvitationId).ToList();
        var distributionResult = ScheduleAppointmentPlanner.DistributeCandidates(slotsResult.Value, eligibleIds, request.ManualAssignments);

        // Unlike Create/Update, a preview still renders a partial/over-capacity plan instead of
        // failing outright - the wizard needs to show the user what's wrong, not just an error code.
        List<(GeneratedSlotDto Slot, Guid? InvitationId)> pairs;
        if (distributionResult.IsSuccess)
        {
            pairs = distributionResult.Value;
        }
        else
        {
            var usedCandidates = new HashSet<Guid>((request.ManualAssignments ?? []).Select(m => m.InvitationId));
            var remaining = new Queue<Guid>(eligibleIds.Where(id => !usedCandidates.Contains(id)));
            pairs = slotsResult.Value
                .Select(slot =>
                {
                    var manual = request.ManualAssignments?.FirstOrDefault(m => m.SlotStartAt == slot.StartAt);
                    if (manual is not null)
                        return (Slot: slot, InvitationId: (Guid?)manual.InvitationId);

                    return (Slot: slot, InvitationId: remaining.Count > 0 ? remaining.Dequeue() : (Guid?)null);
                })
                .ToList();
        }

        var assignedInvitationIds = pairs.Where(p => p.InvitationId is not null).Select(p => p.InvitationId!.Value).ToList();
        var candidateNames = await LoadCandidateNamesAsync(assignedInvitationIds, cancellationToken);

        var slotPreviews = pairs
            .Select(p =>
            {
                string? nameAr = null, nameEn = null;
                if (p.InvitationId is not null && candidateNames.TryGetValue(p.InvitationId.Value, out var names))
                    (nameAr, nameEn) = names;

                return new ScheduleSlotPreviewDto(p.Slot, p.InvitationId, nameAr, nameEn);
            })
            .ToList();

        var unassignedCount = Math.Max(0, eligibleIds.Count - assignedInvitationIds.Count);

        var dto = new SchedulePlanPreviewDto(slotsResult.Value.Count, eligibleIds.Count, unassignedCount, slotPreviews);
        return Result.Ok(dto);
    }

    private async Task<Dictionary<Guid, (string? NameAr, string? NameEn)>> LoadCandidateNamesAsync(
        List<Guid> invitationIds, CancellationToken cancellationToken)
    {
        if (invitationIds.Count == 0)
            return [];

        var invitations = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(i => invitationIds.Contains(i.Id))
            .Select(i => new { i.Id, i.ApplicantId })
            .ToListAsync(cancellationToken);

        var applicantIds = invitations.Select(i => i.ApplicantId).Distinct().ToList();
        var applicants = await userManager.Users
            .Where(u => applicantIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullNameAr, u.FullNameEn })
            .ToListAsync(cancellationToken);

        return invitations.ToDictionary(
            i => i.Id,
            i =>
            {
                var applicant = applicants.FirstOrDefault(a => a.Id == i.ApplicantId);
                return (applicant?.FullNameAr, applicant?.FullNameEn);
            });
    }
}
