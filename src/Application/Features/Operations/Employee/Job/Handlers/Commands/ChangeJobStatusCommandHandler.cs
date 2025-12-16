using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Validations;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class ChangeJobStatusCommandHandler(
    IJobRepository jobRepository,
    IJobValidationService validationService,
    IMediator mediator,
    //INotificationService notificationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeJobStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        ChangeJobStatusCommand request,
        CancellationToken cancellationToken)
    {
        var jobResult = await jobRepository.Repository.GetByIdAsync(request.JobId);
        if (jobResult.IsFailed || jobResult.Value == null)
            return Result.Fail<Unit>(JobMessages.JOB_NOT_FOUND);

        var job = jobResult.Value;

        var validationResult = await validationService.ValidateStatusChange(
            job,
            request.NewStatusId);

        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.Select(e => e.ErrorMessage));

        job.JobStatusId = request.NewStatusId;

        var jobReviewsStatus = await mediator.Send(
            new UpdateJobTabReviewStatusCommand(job.Id),
            cancellationToken);
        var saveResult = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult == 0)
            return Result.Fail<Unit>("Failed to update job status");

        //TODO :: SEND NOTIFICATION
        //await SendNotifications(job, request.NewStatusId);

        return Result.Ok(Unit.Value);
    }

    //TODO :: SEND NOTIFICATION
    //private async Task SendNotifications(JobEntity job, Guid newStatusId)
    //{
    //    switch (newStatusId)
    //    {
    //        case var id when id == JobStatusIds.PendingApproval:
    //            await notificationService.NotifyDepartmentHeadForJobApproval(
    //                job.DepartmentId,
    //                job.Id,
    //                job.TitleAr);
    //            break;

    //        case var id when id == JobStatusIds.Approved:
    //            await notificationService.NotifyJobCreatorOfApproval(
    //                job.CreatedBy,
    //                job.Id,
    //                job.TitleAr);
    //            break;

    //        case var id when id == JobStatusIds.Rejected:
    //            await notificationService.NotifyJobCreatorOfRejection(
    //                job.CreatedBy,
    //                job.Id,
    //                job.TitleAr);
    //            break;
    //    }
    //}
}
