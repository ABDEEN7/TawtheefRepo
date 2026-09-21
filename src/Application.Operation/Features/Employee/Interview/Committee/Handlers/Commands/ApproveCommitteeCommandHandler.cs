using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.Committee.Commands;

namespace Application.Operation.Features.Employee.Interview.Committee.Handlers.Commands;

public sealed class ApproveCommitteeCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<ApproveCommitteeCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ApproveCommitteeCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet;
        var committee = await repo.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (committee is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewCommitteeNotFound));

        _ = Guid.TryParse(currentUserService.UserId, out var approvedById);

        var approveResult = committee.Approve(approvedById, request.DecisionNotes);
        if (approveResult.IsFailed)
            return Result.Fail<Unit>(approveResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
