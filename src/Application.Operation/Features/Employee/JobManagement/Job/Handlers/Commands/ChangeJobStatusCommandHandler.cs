using Application.Operation.Common.Validations;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class ChangeJobStatusCommandHandler(
    IJobRepository jobRepository,
    IJobValidationService validationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ChangeJobStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        ChangeJobStatusCommand request,
        CancellationToken cancellationToken)
    {
        var jobResult = await jobRepository.GetByIdWithDetailsAsync(request.JobId);
        if (jobResult.IsFailed || jobResult.Value == null)
            return Result.Fail<Unit>(JobMessages.JobNotFound);

        var job = jobResult.Value;

        var validationResult = await validationService.ValidateStatusChange(
            job,
            request.NewStatusId);

        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.Select(e => e.ErrorMessage));

        job.ChangeStatus(request.NewStatusId);

        if (request.NewStatusId == JobStatusIds.Closed || request.NewStatusId == JobStatusIds.Cancelled)
        {
            var statusUsed = new List<Guid>
            {
                InvitationStatusIds.Submitted,
                InvitationStatusIds.PendingAttachmentApproval,
                InvitationStatusIds.ReturnedAttachment,
                InvitationStatusIds.Rejected,
                InvitationStatusIds.Closed,
                InvitationStatusIds.Cancelled,
            };
            foreach (var invitation in job.Invitations.Where(i=> !statusUsed.Contains(i.InvitationStatusId)))
                invitation.ChangeInvitationStatus(InvitationStatusIds.Closed);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}

