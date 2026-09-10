using Application.Operation.Features.Interview.Committee.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.Committee.Handlers.Commands;

public sealed class CancelCommitteeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelCommitteeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(CancelCommitteeCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet;
        var committee = await repo.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (committee is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeNotFound));

        var cancelResult = committee.Cancel(request.Reason);
        if (cancelResult.IsFailed)
            return Result.Fail<Unit>(cancelResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
