using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class ApproveTemplateVersionCommandHandler(
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService)
    : IRequestHandler<ApproveTemplateVersionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ApproveTemplateVersionCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet;
        var version = await repo.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        if (version is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionNotFound));

        // Only one version per template can be Approved at a time (DB-enforced), so whichever version
        // currently holds that spot must be superseded before this one can take it - a cross-aggregate
        // concern the handler coordinates, while each version enforces its own transition.
        var previouslyApproved = await repo
            .Where(v => v.InterviewTemplateId == version.InterviewTemplateId
                        && v.Status == TemplateVersionStatus.Approved)
            .ToListAsync(cancellationToken);

        foreach (var previous in previouslyApproved)
        {
            var supersedeResult = previous.Supersede();
            if (supersedeResult.IsFailed)
                return Result.Fail<Unit>(supersedeResult.Errors);
        }

        _ = Guid.TryParse(currentUserService.UserId, out var approvedById);

        var approveResult = version.Approve(approvedById, request.DecisionNotes);
        if (approveResult.IsFailed)
            return Result.Fail<Unit>(approveResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
