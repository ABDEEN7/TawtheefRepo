using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class RemoveTemplateVersionAxisCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveTemplateVersionAxisCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(RemoveTemplateVersionAxisCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewTemplateEvaluationAxis>();
        var versionAxis = await repo.DbSet
            .Include(a => a.InterviewTemplateVersion)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (versionAxis is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionAxisNotFound));

        var editableResult = versionAxis.InterviewTemplateVersion!.EnsureEditable();
        if (editableResult.IsFailed)
            return Result.Fail<Unit>(editableResult.Errors);

        // The FK from criteria to their axis cascades on delete, so removing the axis clears its criteria too.
        await repo.DeleteAsync(versionAxis);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
