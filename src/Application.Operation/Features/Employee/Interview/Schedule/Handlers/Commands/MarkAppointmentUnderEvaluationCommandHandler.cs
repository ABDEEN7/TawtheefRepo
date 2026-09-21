using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class MarkAppointmentUnderEvaluationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<MarkAppointmentUnderEvaluationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(MarkAppointmentUnderEvaluationCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        var result = appointment.MarkUnderEvaluation();
        if (result.IsFailed)
            return Result.Fail<Unit>(result.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewAppointment),
            EntityId = appointment.Id,
            Action = InterviewAppointmentAuditActions.UnderEvaluation
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
