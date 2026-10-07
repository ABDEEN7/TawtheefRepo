using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class SendAppointmentNotificationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SendAppointmentNotificationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SendAppointmentNotificationCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .Include(a => a.InterviewSchedule)
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        var allowed = appointment.InterviewSchedule!.EnsureCanSendNotifications();
        if (allowed.IsFailed)
            return Result.Fail<Unit>(allowed.Errors);

        // Raises AppointmentNotificationSentEvent; its handler queues the candidate's bell/email/SMS
        // inside this save.
        var result = appointment.RecordNotificationSent();
        if (result.IsFailed)
            return Result.Fail<Unit>(result.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
