using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class UpdateTemplateVersionCriterionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateTemplateVersionCriterionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateTemplateVersionCriterionCommand request, CancellationToken cancellationToken)
    {
        var criterion = await unitOfWork.GetEntityRepository<InterviewTemplateEvaluationCriterion>().DbSet
            .Include(c => c.InterviewTemplateEvaluationAxis!)
            .ThenInclude(a => a.InterviewTemplateVersion)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (criterion is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionCriterionNotFound));

        if (request.InterviewEvaluationCriterionId.HasValue)
        {
            var bankCriterionExists = await unitOfWork.GetEntityRepository<InterviewEvaluationCriterion>().DbSet
                .AnyAsync(c => c.Id == request.InterviewEvaluationCriterionId.Value, cancellationToken);
            if (!bankCriterionExists)
                return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewEvaluationCriterionNotFound));
        }

        var updateResult = criterion.Update(
            request.InterviewEvaluationCriterionId,
            request.NameAr,
            request.NameEn,
            request.DescriptionAr,
            request.DescriptionEn,
            request.MaxScore,
            request.IsRequired,
            request.OrderNo,
            request.Notes);
        if (updateResult.IsFailed)
            return Result.Fail<Unit>(updateResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
