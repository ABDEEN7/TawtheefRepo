using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class CancelTemplateVersionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CancelTemplateVersionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(CancelTemplateVersionCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet;
        var version = await repo.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        if (version is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionNotFound));

        var cancelResult = version.Cancel(request.Reason);
        if (cancelResult.IsFailed)
            return Result.Fail<Unit>(cancelResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
