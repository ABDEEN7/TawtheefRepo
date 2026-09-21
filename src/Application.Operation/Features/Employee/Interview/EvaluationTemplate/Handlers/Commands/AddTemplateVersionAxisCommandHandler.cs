using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.EvaluationTemplate.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class AddTemplateVersionAxisCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<AddTemplateVersionAxisCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(AddTemplateVersionAxisCommand request, CancellationToken cancellationToken)
    {
        // The version's own AddAxis() needs its sibling axes in memory to enforce "no duplicate axis per version".
        var version = await unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet
            .Include(v => v.Axes)
            .FirstOrDefaultAsync(v => v.Id == request.InterviewTemplateVersionId, cancellationToken);
        if (version is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewTemplateVersionNotFound));

        // Bank-axis existence is a cross-aggregate lookup, so it stays here rather than inside the entity.
        var axisExists = await unitOfWork.GetEntityRepository<InterviewEvaluationAxis>().DbSet
            .AnyAsync(a => a.Id == request.InterviewEvaluationAxisId, cancellationToken);
        if (!axisExists)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewEvaluationAxisNotFound));

        var addResult = version.AddAxis(request.InterviewEvaluationAxisId, request.MaxScore, request.QualificationScore, request.OrderNo);
        if (addResult.IsFailed)
            return Result.Fail<Guid>(addResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(addResult.Value.Id);
    }
}
