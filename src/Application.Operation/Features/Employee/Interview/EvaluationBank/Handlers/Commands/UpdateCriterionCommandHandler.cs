using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Commands;

public sealed class UpdateCriterionCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateCriterionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateCriterionCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.GetEntityRepository<InterviewEvaluationCriterion>().DbSet;
        var criterion = await repo.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (criterion is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewEvaluationCriterionNotFound));
        }

        var axisExists = await uow.GetEntityRepository<InterviewEvaluationAxis>().DbSet
            .AnyAsync(a => a.Id == request.InterviewEvaluationAxisId, cancellationToken);
        if (!axisExists)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewEvaluationAxisNotFound));
        }

        criterion.InterviewEvaluationAxisId = request.InterviewEvaluationAxisId;
        criterion.NameAr = request.NameAr;
        criterion.NameEn = request.NameEn;
        criterion.DescriptionAr = request.DescriptionAr;
        criterion.DescriptionEn = request.DescriptionEn;
        criterion.IsActive = request.IsActive;

        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
