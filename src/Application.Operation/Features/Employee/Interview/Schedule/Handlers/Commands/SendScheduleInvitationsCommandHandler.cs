using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class SendScheduleInvitationsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SendScheduleInvitationsCommand, IResult<int>>
{
    public async Task<IResult<int>> Handle(SendScheduleInvitationsCommand request, CancellationToken cancellationToken)
    {
        var schedule = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .FirstOrDefaultAsync(s => s.Id == request.ScheduleId, cancellationToken);
        if (schedule is null)
            return Result.Fail<int>(new Error(ErrorsCodes.InterviewScheduleNotFound));

        var allowed = schedule.EnsureCanSendNotifications();
        if (allowed.IsFailed)
            return Result.Fail<int>(allowed.Errors);

        // Same rule as InterviewAppointment.CanBeNotified, restricted to first-time sends.
        var pending = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .Where(a => a.InterviewScheduleId == schedule.Id
                        && a.InvitationId != null
                        && a.Status == AppointmentStatus.Scheduled
                        && a.AttendanceStatus == null
                        && a.InvitationSentAt == null)
            .ToListAsync(cancellationToken);
        if (pending.Count == 0)
            return Result.Ok(0);

        // Each call raises AppointmentNotificationSentEvent; their handlers queue the candidates'
        // bell/email/SMS inside this single save.
        foreach (var appointment in pending)
        {
            var result = appointment.RecordNotificationSent();
            if (result.IsFailed)
                return Result.Fail<int>(result.Errors);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(pending.Count);
    }
}
