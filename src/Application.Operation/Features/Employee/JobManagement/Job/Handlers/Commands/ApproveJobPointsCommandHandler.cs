using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.Utilities;
using Cortex.Mediator.Commands;
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
    IUnitOfWork uow) : ICommandHandler<ApproveJobPointsCommand, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(ApproveJobPointsCommand request, CancellationToken cancellationToken)
    {
        var jobStatus = await uow.GetEntityRepository<JobEntity>()
            .DbSet
            .AsNoTracking()
            .Where(x => x.Id == request.JobId)
            .Select(x => x.JobStatusId)
            .FirstOrDefaultAsync(cancellationToken);

        if (jobStatus == Guid.Empty)
            return Result.Fail<bool>(JobMessages.JobNotFound);

        if (jobStatus != JobStatusIds.Approved)
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

        var result = await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(result > 0);
    }
}
