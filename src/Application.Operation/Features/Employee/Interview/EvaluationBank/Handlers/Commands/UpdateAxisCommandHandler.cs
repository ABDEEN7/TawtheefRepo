using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Commands;

public sealed class UpdateAxisCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAxisCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateAxisCommand request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetEntityRepository<InterviewEvaluationAxis>().DbSet;
        var axis = await repo.FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if(axis is null)
        {
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewEvaluationAxisNotFound));
        }

        // Update the axis properties
        axis.NameAr = request.NameAr;
        axis.NameEn = request.NameEn;
        axis.DescriptionAr = request.DescriptionAr;
        axis.DescriptionEn = request.DescriptionEn;
        axis.IsActive = request.IsActive;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
