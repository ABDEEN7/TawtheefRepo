using Application.Operation.Features.Interview.Committee.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.Committee.Handlers.Commands;

public sealed class StopCommitteeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<StopCommitteeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(StopCommitteeCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet;
        var committee = await repo.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (committee is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeNotFound));

        var stopResult = committee.Stop(request.Reason);
        if (stopResult.IsFailed)
            return Result.Fail<Unit>(stopResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
