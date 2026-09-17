using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Commands;

public sealed class CreateCriterionCommandHandler(IUnitOfWork uow)
    : IRequestHandler<CreateCriterionCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateCriterionCommand request, CancellationToken cancellationToken)
    {
        var axisExists = await uow.GetEntityRepository<InterviewEvaluationAxis>().DbSet
            .AnyAsync(a => a.Id == request.InterviewEvaluationAxisId, cancellationToken);
        if (!axisExists)
        {
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewEvaluationAxisNotFound));
        }

        var criterion = new InterviewEvaluationCriterion
        {
            InterviewEvaluationAxisId = request.InterviewEvaluationAxisId,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive
        };

        await uow.GetEntityRepository<InterviewEvaluationCriterion>().AddAsync(criterion);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok(criterion.Id);
    }
}
