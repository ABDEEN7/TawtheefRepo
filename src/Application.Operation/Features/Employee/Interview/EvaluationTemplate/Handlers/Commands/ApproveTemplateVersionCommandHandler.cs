using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Commands;

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

        if (version.Status != TemplateVersionStatus.PendingApproval)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionNotPendingApproval));

        // Only one version per template can be Approved at a time (DB-enforced by UX_TemplateVersion_Approved,
        // a filtered unique index on InterviewTemplateId), so whichever version currently holds that spot
        // must be superseded - and that change committed to the database - before this one can take it.
        var previouslyApproved = await repo
            .Where(v => v.InterviewTemplateId == version.InterviewTemplateId
                        && v.Status == TemplateVersionStatus.Approved)
            .ToListAsync(cancellationToken);

        _ = Guid.TryParse(currentUserService.UserId, out var approvedById);

        return await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            // roles : cuz of UX_approved version per template : 
            // Separate SaveChanges calls ensure the old version is Superseded before the new version becomes Approved, avoiding the unique index conflict.
            //Both operations are inside the same transaction, so they still commit or roll back together.
            foreach (var previous in previouslyApproved)
            {
                var supersedeResult = previous.Supersede();
                if (supersedeResult.IsFailed)
                    throw new InvalidOperationException(
                        $"Unexpected failure superseding interview template version {previous.Id}: " +
                        string.Join("; ", supersedeResult.Errors.Select(e => e.Message)));
            }

            if (previouslyApproved.Count > 0)
                await unitOfWork.SaveChangesAsync(ct);

            var approveResult = version.Approve(approvedById, request.DecisionNotes);
            if (approveResult.IsFailed)
                throw new InvalidOperationException(
                    $"Unexpected failure approving interview template version {version.Id}: " +
                    string.Join("; ", approveResult.Errors.Select(e => e.Message)));

            await unitOfWork.SaveChangesAsync(ct);

            return Result.Ok(Unit.Value);
        }, cancellationToken);
    }
}
