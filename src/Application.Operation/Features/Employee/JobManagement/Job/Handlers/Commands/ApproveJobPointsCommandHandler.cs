using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.Utilities;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class ApproveJobPointsCommandHandler(
    IJobPointsRepository jobPointsRepository,
    IJobPointsConfigurationsRepository jobPointsConfigurationsRepository,
    IUnitOfWork uow) : IRequestHandler<ApproveJobPointsCommand, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(ApproveJobPointsCommand request, CancellationToken cancellationToken)
    {
        var jobRepo = uow.GetEntityRepository<JobEntity>();
        var jobResult = await jobRepo.GetByIdAsync(request.JobId, cancellationToken);
        if (jobResult.IsFailed || jobResult.Value is null) return Result.Fail<bool>(JobMessages.JobNotFound);

        var job = jobResult.Value;
        if (job.JobStatusId != JobStatusIds.PendingPointApproval)
            return Result.Fail<bool>(JobMessages.JobPointsJobNotApproved);

        var jobPoints = await jobPointsRepository.GetByJobIdAsync(request.JobId);
        if (jobPoints.IsFailed)
            return Result.Fail<bool>(JobMessages.JobPointsNotFound);

        var points = jobPoints.Value;
        if (points.IsApproved)
            return Result.Fail<bool>(JobMessages.JobPointsAlreadyApproved);

        var configResult = await jobPointsConfigurationsRepository.GetAsync();
        if (configResult.IsFailed)
            return Result.Fail<bool>(JobMessages.JobPointsConfigurationNotFound);

        var validationResult = JobPointsValidationUtility.Validate(points, configResult.Value);
        if (validationResult.IsFailed)
            return Result.Fail<bool>(validationResult.Errors);
        points.IsApproved = true;

        job.ChangeStatus(JobStatusIds.ReadyForAnnouncement);
        var result = await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(result > 0);
    }
}

