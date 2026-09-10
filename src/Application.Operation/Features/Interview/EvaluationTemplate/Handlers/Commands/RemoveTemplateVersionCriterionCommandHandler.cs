using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class RemoveTemplateVersionCriterionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveTemplateVersionCriterionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RemoveTemplateVersionCriterionCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewTemplateEvaluationCriterion>();
        var criterion = await repo.DbSet
            .Include(c => c.InterviewTemplateEvaluationAxis!)
            .ThenInclude(a => a.InterviewTemplateVersion)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (criterion is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionCriterionNotFound));

        var editableResult = criterion.InterviewTemplateEvaluationAxis!.InterviewTemplateVersion!.EnsureEditable();
        if (editableResult.IsFailed)
            return Result.Fail<Unit>(editableResult.Errors);

        await repo.DeleteAsync(criterion);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
