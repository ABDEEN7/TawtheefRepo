using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.Interfaces;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class ChangeJobStatusCommandHandler(
    IJobRepository jobRepository,
    IJobValidationService validationService,
    //INotificationService notificationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeJobStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        ChangeJobStatusCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Get existing job
        var jobResult = await jobRepository.GetByIdWithDetailsAsync(request.JobId);
        if (jobResult.IsFailed || jobResult.Value == null)
            return Result.Fail<Unit>(JobValidationMessages.JOB_NOT_FOUND);

        var job = jobResult.Value;

        // 2. Validate status change
        var validationResult = await validationService.ValidateStatusChange(
            job,
            request.NewStatusId);

        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.Select(e => e.ErrorMessage));

        // 3. Update job status (BRD: تتحول حالة الوظيفة تلقائيًا)
        job.JobStatusId = request.NewStatusId;

        // 4. Handle specific status transitions (BRD متطلبات خاصة)
        await HandleSpecificStatusActions(job, request.NewStatusId);

        // 5. Save changes
        var saveResult = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult == 0)
            return Result.Fail<Unit>("Failed to update job status");

        // 6. Send notifications if needed (BRD: يتم إرسال إشعار لرئيس القسم)
        //await SendNotifications(job, request.NewStatusId);

        return Result.Ok(Unit.Value);
    }

    private Task HandleSpecificStatusActions(
        JobEntity job,
        Guid newStatusId)
    {
        switch (newStatusId)
        {
            case var id when id == JobStatusIds.PendingApproval:
                job.UpdatedDate = DateTime.UtcNow;
                break;

            case var id when id == JobStatusIds.Approved:
                job.UpdatedDate = DateTime.UtcNow;
                break;

            case var id when id == JobStatusIds.Published:
                job.UpdatedDate = DateTime.UtcNow;
                break;

            case var id when id == JobStatusIds.Closed:
                job.UpdatedDate = DateTime.UtcNow;
                break;

            case var id when id == JobStatusIds.Cancelled:
                job.UpdatedDate = DateTime.UtcNow;
                break;

            case var id when id == JobStatusIds.Rejected:
                job.UpdatedDate = DateTime.UtcNow;
                break;
        }

        return Task.CompletedTask;
    }
    //TODO :: SEND NOTIFICATION
    //private async Task SendNotifications(JobEntity job, Guid newStatusId)
    //{
    //    switch (newStatusId)
    //    {
    //        case var id when id == JobStatusIds.PendingApproval:
    //            // BRD: يتم إرسال إشعار لرئيس القسم بوجود وظيفة جديدة تحتاج للمراجعة
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
