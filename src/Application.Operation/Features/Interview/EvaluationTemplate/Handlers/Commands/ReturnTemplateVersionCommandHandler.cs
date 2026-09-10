using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class ReturnTemplateVersionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ReturnTemplateVersionCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ReturnTemplateVersionCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewTemplateVersion>().DbSet;
        var version = await repo.FirstOrDefaultAsync(v => v.Id == request.Id, cancellationToken);
        if (version is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateVersionNotFound));

        var returnResult = version.Return(request.Reason);
        if (returnResult.IsFailed)
            return Result.Fail<Unit>(returnResult.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
