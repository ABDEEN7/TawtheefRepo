using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class UpdateTemplateVersionAxisCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateTemplateVersionAxisCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateTemplateVersionAxisCommand request, CancellationToken cancellationToken)
    {
        var versionAxis = await unitOfWork.GetEntityRepository<InterviewTemplateEvaluationAxis>().DbSet
            .Include(a => a.InterviewTemplateVersion)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (versionAxis is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionAxisNotFound));

        var updateResult = versionAxis.UpdateScore(request.MaxScore, request.QualificationScore, request.OrderNo);
        if (updateResult.IsFailed)
            return Result.Fail<Unit>(updateResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
