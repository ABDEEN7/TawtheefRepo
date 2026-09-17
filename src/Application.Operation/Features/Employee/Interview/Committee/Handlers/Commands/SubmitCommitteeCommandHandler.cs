using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.Committee.Commands;

namespace Application.Operation.Features.Employee.Interview.Committee.Handlers.Commands;

public sealed class SubmitCommitteeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<SubmitCommitteeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SubmitCommitteeCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet;
        var committee = await repo.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (committee is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeNotFound));

        var submitResult = committee.Submit();
        if (submitResult.IsFailed)
            return Result.Fail<Unit>(submitResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
