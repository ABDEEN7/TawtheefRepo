using Application.Operation.Common.Repositories;
using Cortex.Mediator.Notifications;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Common.Events.Job;

public sealed class JobStatusChangedDomainEventHandler(
    IUnitOfWork uow,
    IJobTabReviewNoteRepository jobTabReviewNoteRepository)
    : INotificationHandler<JobStatusChangedDomainEvent>
{
    public async Task Handle(JobStatusChangedDomainEvent notification, CancellationToken ct)
    {
        var reviewsResult = await jobTabReviewNoteRepository
            .GetByIdWithDetailsAsync(notification.JobId);

        if (reviewsResult.IsFailed || reviewsResult.Value is null || reviewsResult.Value.Count == 0)
            return;

        reviewsResult.Value.ForEach(r => r.IsResolved = true);
        await uow.SaveChangesAsync(ct);
    }
}
