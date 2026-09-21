using Application.Operation.Features.Employee.Interview.Schedule.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Schedule.Handlers.Commands;

public sealed class SubmitScheduleCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitScheduleCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SubmitScheduleCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet;
        var schedule = await repo.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (schedule is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewScheduleNotFound));

        var submitResult = schedule.Submit();
        if (submitResult.IsFailed)
            return Result.Fail<Unit>(submitResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
