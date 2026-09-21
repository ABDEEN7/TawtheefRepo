using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class MarkScheduleReadyForExecutionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<MarkScheduleReadyForExecutionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(MarkScheduleReadyForExecutionCommand request, CancellationToken cancellationToken)
    {
        var schedule = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (schedule is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewScheduleNotFound));

        var result = schedule.MarkReadyForExecution();
        if (result.IsFailed)
            return Result.Fail<Unit>(result.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewSchedule),
            EntityId = schedule.Id,
            Action = InterviewScheduleAuditActions.ReadyForExecution
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
