using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class AddTemplateVersionCriterionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddTemplateVersionCriterionCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(AddTemplateVersionCriterionCommand request, CancellationToken cancellationToken)
    {
        var versionAxis = await unitOfWork.GetEntityRepository<InterviewTemplateEvaluationAxis>().DbSet
            .Include(a => a.InterviewTemplateVersion)
            .FirstOrDefaultAsync(a => a.Id == request.InterviewTemplateEvaluationAxisId, cancellationToken);
        if (versionAxis is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewTemplateVersionAxisNotFound));

        // Bank-criterion existence is a cross-aggregate lookup, so it stays here rather than inside the entity.
        if (request.InterviewEvaluationCriterionId.HasValue)
        {
            var bankCriterionExists = await unitOfWork.GetEntityRepository<InterviewEvaluationCriterion>().DbSet
                .AnyAsync(c => c.Id == request.InterviewEvaluationCriterionId.Value, cancellationToken);
            if (!bankCriterionExists)
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewEvaluationCriterionNotFound));
        }

        var addResult = versionAxis.AddCriterion(
            request.InterviewEvaluationCriterionId,
            request.NameAr,
            request.NameEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.MaxScore,
            request.IsRequired,
            request.OrderNo,
            request.Notes);
        if (addResult.IsFailed)
            return Result.Fail<Guid>(addResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(addResult.Value.Id);
    }
}
