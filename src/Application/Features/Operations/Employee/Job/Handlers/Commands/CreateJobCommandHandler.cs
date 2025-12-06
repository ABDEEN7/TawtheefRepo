using FluentResults;
using Mapster;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class CreateJobCommandHandler(
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        
        var job = request.Job.Adapt<JobEntity>();
        var result = await jobRepository.Repository.AddAsync(job);
        if (result.IsFailed)
            return Result.Fail<Guid>(result.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(job.Id);
    }
}
