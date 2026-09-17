using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;
using Application.Operation.Features.Employee.Interview.EvaluationBank.Commands;

namespace Application.Operation.Features.Employee.Interview.EvaluationBank.Handlers.Commands;

public sealed class CreateAxisCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateAxisCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateAxisCommand request, CancellationToken cancellationToken)
    {
        var axis = new InterviewEvaluationAxis
        {
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive
        };

       await unitOfWork.GetEntityRepository<InterviewEvaluationAxis>().AddAsync(axis);
       await  unitOfWork.SaveChangesAsync(cancellationToken);

       return Result.Ok(axis.Id);
    }
}
