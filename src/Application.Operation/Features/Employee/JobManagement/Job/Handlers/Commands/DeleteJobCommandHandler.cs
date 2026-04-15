using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Events.Operation.Employee.Job;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class DeleteJobCommandHandler(IJobRepository jobRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {
        var existingJobResult = await jobRepository.GetByIdWithDetailsAsync(request.JobId, cancellationToken);
        if (existingJobResult.IsFailed)
            return Result.Fail<Unit>(existingJobResult.Errors);

        var existingJob = existingJobResult.Value;

        existingJob.AddDomainEvent(new JobDeletedDomainEvent(existingJob, DateTimeOffset.UtcNow));
        
        if (existingJob.JobPoints is not null)
        {
            unitOfWork.RemoveRange(existingJob.JobPoints.Details.ToList());
            unitOfWork.Remove(existingJob.JobPoints);
        }

        if (existingJob.CandidateFilterSetting is not null)
        {
            unitOfWork.RemoveRange(existingJob.CandidateFilterSetting.CandidateTypePercentages.ToList());
            unitOfWork.RemoveRange(existingJob.CandidateFilterSetting.NationalityPercentages.ToList());
            unitOfWork.Remove(existingJob.CandidateFilterSetting);
        }

        if (existingJob.ReviewAttachment is not null)
        {
            unitOfWork.Remove(existingJob.ReviewAttachment);
        }

        var invitationHistories = existingJob.Invitations
            .SelectMany(invitation => invitation.History)
            .ToList();
        unitOfWork.RemoveRange(invitationHistories);
        unitOfWork.RemoveRange(existingJob.Invitations);
        unitOfWork.RemoveRange(existingJob.JobDegrees);
        unitOfWork.RemoveRange(existingJob.JobConditions);
        unitOfWork.RemoveRange(existingJob.JobSkills);
        unitOfWork.RemoveRange(existingJob.JobResponsibilities);
        unitOfWork.RemoveRange(existingJob.JobRequiredAttachments);
        unitOfWork.RemoveRange(existingJob.TabReviewNotes);

        unitOfWork.Remove(existingJob);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);  
    }
    
}

