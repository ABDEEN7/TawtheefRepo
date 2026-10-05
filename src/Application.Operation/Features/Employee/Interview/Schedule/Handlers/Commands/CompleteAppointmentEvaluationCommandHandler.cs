using Application.Operation.Features.Employee.Interview.ResultReport.Services;
using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class CompleteAppointmentEvaluationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CompleteAppointmentEvaluationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(CompleteAppointmentEvaluationCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        var result = appointment.CompleteEvaluation();
        if (result.IsFailed)
            return Result.Fail<Unit>(result.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewAppointment),
            EntityId = appointment.Id,
            Action = InterviewAppointmentAuditActions.EvaluationCompleted
        }, cancellationToken);

        // Manual override of the quorum auto-complete - same "was this the last one?" check as Submit.
        var generateResult = await InterviewResultCalculationService.TryGenerateReportIfScheduleDoneAsync(
            unitOfWork, appointment.InterviewScheduleId, cancellationToken);
        if (generateResult.IsFailed)
            return Result.Fail<Unit>(generateResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
