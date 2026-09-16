using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class CloseScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CloseScheduleCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(CloseScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (schedule is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewScheduleNotFound));

        var result = schedule.Close();
        if (result.IsFailed)
            return Result.Fail<Unit>(result.Errors);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewSchedule),
            EntityId = schedule.Id,
            Action = InterviewScheduleAuditActions.Closed
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
