using System;
using System.Collections.Generic;
using System.Text;
using Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;


namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Commands;

public sealed class ChangeCriterionActivationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeCriterionActivationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeCriterionActivationCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewEvaluationCriterion>().DbSet;
        var criterion = await repo.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (criterion is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewEvaluationCriterionNotFound));
        }
        criterion.IsActive = request.IsActive;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
