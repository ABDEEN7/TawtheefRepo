using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class UpdateScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateScheduleCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var scheduleRepo = unitOfWork.GetEntityRepository<InterviewSchedule>();

        var schedule = await scheduleRepo.DbSet
            .Include(s => s.Appointments)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (schedule is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewScheduleNotFound));

        var committeeResult = await ScheduleEligibilityResolver.GetApprovedCommitteeAsync(unitOfWork, schedule.JobId, cancellationToken);
        if (committeeResult.IsFailed)
            return Result.Fail<Unit>(committeeResult.Errors);
        var committee = committeeResult.Value;

        var templateResult = await ScheduleEligibilityResolver.EnsureTemplateHasApprovedVersionAsync(
            unitOfWork, committee.InterviewTemplateId, cancellationToken);
        if (templateResult.IsFailed)
            return Result.Fail<Unit>(templateResult.Errors);

        // Editing regenerates the appointment list from scratch (schedule.md). This schedule's own
        // current appointments are still in the DB at this point (deleted below, right before
        // SaveChanges) - both checks explicitly exclude them by Id rather than relying on delete
        // ordering, since a plain query never sees not-yet-saved changes anyway.
        var eligibleCandidates = await ScheduleEligibilityResolver.GetEligibleCandidatesAsync(
            unitOfWork, schedule.JobId, cancellationToken, excludeScheduleId: schedule.Id);
        if (eligibleCandidates.Count == 0)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewScheduleNoEligibleCandidates));

        var slotsResult = ScheduleAppointmentPlanner.GenerateSlots(
            request.Periods, request.InterviewType, request.DurationMinutes, request.BufferMinutes);
        if (slotsResult.IsFailed)
            return Result.Fail<Unit>(slotsResult.Errors);

        var conflictResult = await ScheduleConflictChecker.ValidateNoConflictsAsync(
            unitOfWork, committee.Id, slotsResult.Value, cancellationToken, excludeScheduleId: schedule.Id);
        if (conflictResult.IsFailed)
            return Result.Fail<Unit>(conflictResult.Errors);

        var distributionResult = ScheduleAppointmentPlanner.DistributeCandidates(
            slotsResult.Value, eligibleCandidates.Select(c => c.InvitationId).ToList(), request.ManualAssignments);
        if (distributionResult.IsFailed)
            return Result.Fail<Unit>(distributionResult.Errors);

        var oldSnapshot = new
        {
            schedule.TitleAr,
            schedule.TitleEn,
            schedule.DefaultInterviewType,
            schedule.DefaultDurationMinutes,
            schedule.DefaultBufferMinutes,
            AppointmentCount = schedule.Appointments.Count
        };

        var updateResult = schedule.Update(
            request.TitleAr, request.TitleEn, request.InterviewType, request.DurationMinutes, request.BufferMinutes);
        if (updateResult.IsFailed)
            return Result.Fail<Unit>(updateResult.Errors);

        // All prior validation above already ran with this schedule's own rows excluded - safe to
        // clear them now and attach the freshly generated set in the same SaveChanges.
        await unitOfWork.GetEntityRepository<InterviewAppointment>().DeleteRangeAsync(schedule.Appointments);
        schedule.Appointments.Clear();

        foreach (var (slot, invitationId) in distributionResult.Value)
        {
            schedule.AddAppointment(
                committee.Id, request.InterviewType, slot.RoomId, slot.RemoteMeetingUrl, slot.RemoteMeetingInstructions,
                slot.StartAt, slot.EndAt, invitationId);
        }

        var proposeResult = schedule.Propose();
        if (proposeResult.IsFailed)
            return Result.Fail<Unit>(proposeResult.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewSchedule),
            EntityId = schedule.Id,
            Action = InterviewScheduleAuditActions.Resubmitted,
            OldValues = JsonSerializer.Serialize(oldSnapshot),
            NewValues = JsonSerializer.Serialize(new
            {
                schedule.TitleAr,
                schedule.TitleEn,
                schedule.DefaultInterviewType,
                schedule.DefaultDurationMinutes,
                schedule.DefaultBufferMinutes,
                AppointmentCount = schedule.Appointments.Count
            })
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
