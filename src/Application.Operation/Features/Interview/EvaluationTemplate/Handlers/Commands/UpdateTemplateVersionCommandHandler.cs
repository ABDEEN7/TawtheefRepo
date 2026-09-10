using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class UpdateTemplateVersionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateTemplateVersionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateTemplateVersionCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet;
        var version = await repo.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        if (version is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionNotFound));

        var updateResult = version.UpdateScoring(request.FinalScore, request.QualificationScore, request.CalculationMethod);
        if (updateResult.IsFailed)
            return Result.Fail<Unit>(updateResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
