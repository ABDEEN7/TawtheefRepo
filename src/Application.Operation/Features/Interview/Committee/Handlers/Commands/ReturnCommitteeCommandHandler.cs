using Application.Operation.Features.Interview.Committee.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.Committee.Handlers.Commands;

public sealed class ReturnCommitteeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ReturnCommitteeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReturnCommitteeCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet;
        var committee = await repo.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (committee is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeNotFound));

        var returnResult = committee.Return(request.Reason);
        if (returnResult.IsFailed)
            return Result.Fail<Unit>(returnResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
