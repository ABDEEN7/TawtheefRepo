using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Cortex.Mediator.Commands;
using FluentResults;
using FluentValidation;
using Mapster;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Events.Operation.Employee.Job;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class CreateJobCommandHandler(
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateJobCommand> validator)
    : ICommandHandler<CreateJobCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail<Guid>(
                validationResult.Errors.Select(e => e.ErrorMessage)
            );
        }
        var job = request.Job.Adapt<JobEntity>();
        job.ChangeStatus(JobStatusIds.Draft);
        job.AddDomainEvent(new JobCreatedDomainEvent(job, DateTimeOffset.UtcNow));
        var result = await jobRepository.Repository.AddAsync(job);
        if (result.IsFailed)
            return Result.Fail<Guid>(result.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(job.Id);
    }
}
