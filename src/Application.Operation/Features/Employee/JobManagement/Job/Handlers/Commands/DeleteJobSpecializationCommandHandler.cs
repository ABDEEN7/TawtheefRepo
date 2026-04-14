using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class DeleteJobSpecializationCommandHandler(
    IJobSpecializationRepository jobSpecializationRepository,
    IUnitOfWork unitOfWork
    ) : IRequestHandler<DeleteJobSpecializationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteJobSpecializationCommand request, CancellationToken cancellationToken)
    {
        var spec = await jobSpecializationRepository.Repository.GetByIdAsync(request.SpecializationId, cancellationToken);
        if (spec.IsFailed || spec.Value == null)
            return Result.Fail<Unit>("Specialization not found");

        await jobSpecializationRepository.Repository.DeleteAsync(spec.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
