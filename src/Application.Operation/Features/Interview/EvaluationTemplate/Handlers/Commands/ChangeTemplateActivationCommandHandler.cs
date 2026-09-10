using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class ChangeTemplateActivationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeTemplateActivationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeTemplateActivationCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet;
        var template = await repo.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (template is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewTemplateNotFound));

        template.IsActive = request.IsActive;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
