using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using Application.Operation.Features.Employee.Interview.Schedule.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class CreateScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateScheduleCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var jobExists = await unitOfWork.GetEntityRepository<Job>().DbSet
            .AnyAsync(j => j.Id == request.JobId, cancellationToken);
        if (!jobExists)
            return Result.Fail<Guid>(new Error(ErrorsCodes.JobNotFound));

        var committeeResult = await ScheduleEligibilityResolver.GetApprovedCommitteeAsync(unitOfWork, request.JobId, cancellationToken);
        if (committeeResult.IsFailed)
            return Result.Fail<Guid>(committeeResult.Errors);
        var committee = committeeResult.Value;

        var templateResult = await ScheduleEligibilityResolver.EnsureTemplateHasApprovedVersionAsync(
            unitOfWork, committee.InterviewTemplateId, cancellationToken);
        if (templateResult.IsFailed)
            return Result.Fail<Guid>(templateResult.Errors);

        var eligibleCandidates = await ScheduleEligibilityResolver.GetEligibleCandidatesAsync(unitOfWork, request.JobId, cancellationToken);
        if (eligibleCandidates.Count == 0)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewScheduleNoEligibleCandidates));

        var slotsResult = ScheduleAppointmentPlanner.GenerateSlots(
            request.Periods, request.InterviewType, request.DurationMinutes, request.BufferMinutes);
        if (slotsResult.IsFailed)
            return Result.Fail<Guid>(slotsResult.Errors);

        var conflictResult = await ScheduleConflictChecker.ValidateNoConflictsAsync(
            unitOfWork, committee.Id, slotsResult.Value, cancellationToken);
        if (conflictResult.IsFailed)
            return Result.Fail<Guid>(conflictResult.Errors);

        var distributionResult = ScheduleAppointmentPlanner.DistributeCandidates(
            slotsResult.Value, eligibleCandidates.Select(c => c.InvitationId).ToList(), request.ManualAssignments);
        if (distributionResult.IsFailed)
            return Result.Fail<Guid>(distributionResult.Errors);

        var schedule = InterviewSchedule.Create(
            request.JobId, committee.InterviewTemplateId, request.TitleAr, request.TitleEn,
            request.InterviewType, request.DurationMinutes, request.BufferMinutes);

        await unitOfWork.GetEntityRepository<InterviewSchedule>().AddAsync(schedule, cancellationToken);

        foreach (var (slot, invitationId) in distributionResult.Value)
        {
            schedule.AddAppointment(
                committee.Id, request.InterviewType, slot.RoomId, slot.RemoteMeetingUrl, slot.RemoteMeetingInstructions,
                slot.StartAt, slot.EndAt, invitationId);
        }

        var proposeResult = schedule.Propose();
        if (proposeResult.IsFailed)
            return Result.Fail<Guid>(proposeResult.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewSchedule),
            EntityId = schedule.Id,
            Action = InterviewScheduleAuditActions.Created,
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

        return Result.Ok(schedule.Id);
    }
}
