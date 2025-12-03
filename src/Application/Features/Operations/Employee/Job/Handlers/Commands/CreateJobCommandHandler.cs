using FluentResults;
using Mapster;
using MediatR;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class CreateJobCommandHandler(
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate first
        var duplicateCheck = await CheckForDuplicateJob(request.Job, cancellationToken);
        if (duplicateCheck.IsFailed)
        {
            return Result.Fail<Guid>(duplicateCheck.Errors);
        }

        var job = request.Job.Adapt<JobEntity>();
        var result = await jobRepository.Repository.AddAsync(job);
        if (result.IsFailed)
            return Result.Fail<Guid>(result.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(job.Id);
    }

    private async Task<Result> CheckForDuplicateJob(CreateJobDto jobDto, CancellationToken cancellationToken)
    {
        var jobsResult = await jobRepository.Repository.GetAllAsync();
        if (jobsResult.IsFailed || jobsResult.Value is null)
        {
            return Result.Fail(JobValidationMessages.DUPLICATE_JOB);
        }

        var duplicateExists = jobsResult.Value
            .Any(j =>
                j.Title == jobDto.Title &&
                j.RequestingDepartmentId == jobDto.RequestingDepartmentId &&
                j.JobCategoryId == jobDto.JobCategoryId &&
                j.MajorId == jobDto.MajorId &&
                !j.IsDeleted);

        return duplicateExists
            ? Result.Fail(JobValidationMessages.DUPLICATE_JOB)
            : Result.Ok();
    }
}
