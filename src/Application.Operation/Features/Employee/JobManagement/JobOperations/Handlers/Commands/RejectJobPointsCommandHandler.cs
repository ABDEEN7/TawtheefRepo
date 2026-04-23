using Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Commands;

public sealed class RejectJobPointsCommandHandler(
    IUnitOfWork uow
) : IRequestHandler<RejectJobPointsCommand, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(RejectJobPointsCommand request, CancellationToken cancellationToken)
    {
        var jobRepo = uow.GetEntityRepository<JobEntity>();
        var jobResult = await jobRepo.GetByIdAsync(request.JobId, cancellationToken);
        if (jobResult.IsFailed || jobResult.Value is null) return Result.Fail<bool>(JobMessages.JobNotFound);

        var job = jobResult.Value;
        if (job.JobStatusId != JobStatusIds.PendingPointApproval)
            return Result.Fail<bool>(JobMessages.JobPointsNotPendingApproval);

        var pointsRepo = uow.GetEntityRepository<JobPointsMain>();
        var points = await pointsRepo.DbSet.FirstOrDefaultAsync(x => x.JobId == request.JobId, cancellationToken);
        
        if (points == null) return Result.Fail<bool>(JobMessages.JobPointsNotFound);
        if (points.IsApproved) return Result.Fail<bool>(JobMessages.JobPointsAlreadyApproved);

        points.IsApproved = false;
        
        // Update job status back to NeedPointUpdate so it can be edited
        job.ChangeStatus(JobStatusIds.NeedPointUpdate);

        // Add domain event for notification
        job.AddDomainEvent(new JobPointsRejectedDomainEvent(job, request.Reason, DateTime.Now));

        var result = await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(result > 0);
    }
}
