using Application.Operation.Features.Interview.Committee.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.Committee.Handlers.Commands;

public sealed class UpdateCommitteeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCommitteeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateCommitteeCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet;
        var committee = await repo.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (committee is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeNotFound));

        var updateResult = committee.Update(request.NameAr, request.NameEn, request.ScopeDescription, request.Notes);
        if (updateResult.IsFailed)
            return Result.Fail<Unit>(updateResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
