using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Commands;

public sealed class SendJobCandidateInvitationsCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ISmsSender smsSender,
    IJobPointsRepository jobPointsRepository)
    : IRequestHandler<SendJobCandidateInvitationsCommand, IResult<SendJobCandidateInvitationsResult>>
{
    private readonly JobCandidatePointsCalculator _pointsCalculator = new();

    public async Task<IResult<SendJobCandidateInvitationsResult>> Handle(
    SendJobCandidateInvitationsCommand request,
    CancellationToken cancellationToken)
{
    var query = JobCandidatesQueryBuilder.Build(unitOfWork, request.JobId, request.Filter);

    // Restrict to selected applicants if provided
    if (request.ApplicantIds is { Count: > 0 })
    {
        query = query.Where(c => request.ApplicantIds.Contains(c.ApplicantId));
    }

    var candidates = await query.ToListAsync(cancellationToken);

    // Only calculate points if minimum points filter is used
    if (request.Filter?.MinimumPoints is { } minPoints)
    {
        var jobPoints = await LoadJobPointsAsync(request.JobId);

        candidates = candidates
            .Select(c => c with { Points = _pointsCalculator.Calculate(c, jobPoints) })
            .Where(c => c.Points >= minPoints)
            .ToList();
    }

    if (candidates.Count == 0)
    {
        return Result.Ok(new SendJobCandidateInvitationsResult
        {
            TotalTargets = 0,
            SentEmailCount = 0,
            SentSmsCount = 0,
            UpdatedStatusCount = 0
        });
    }

    var job = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

    var jobTitle = job?.TitleEn ?? job?.TitleAr ?? string.Empty;

    // Create invitations for all returned candidates (none are invited yet)
    var invitationsRepo = unitOfWork.GetEntityRepository<Invitation>().DbSet;

    var newInvitations = candidates.Select(c => new Invitation
    {
        Id = Guid.NewGuid(),
        JobId = request.JobId,
        ApplicantId = c.ApplicantId,
        InvitationStatusId = InvitationStatusIds.NewInvitation,
        CreatedDate = DateTime.UtcNow
    }).ToList();

    await invitationsRepo.AddRangeAsync(newInvitations, cancellationToken);

    // Send email/sms
    var sentEmailCount = 0;
    var sentSmsCount = 0;

    foreach (var candidate in candidates)
    {
        var body = string.IsNullOrWhiteSpace(jobTitle)
            ? "You have been invited to apply for a job on Tawtheef."
            : $"You have been invited to apply for {jobTitle} on Tawtheef.";

        if (!string.IsNullOrWhiteSpace(candidate.Applicant?.Email))
        {
            var emailResult = await emailSender.SendAsync(
                candidate.Applicant.Email,
                "Job Invitation",
                body,
                cancellationToken);

            if (emailResult.ok) sentEmailCount++;
        }

        if (!string.IsNullOrWhiteSpace(candidate.Applicant?.PhoneNumber))
        {
            var smsResult = await smsSender.SendAsync(
                candidate.Applicant.PhoneNumber,
                body,
                cancellationToken);

            if (smsResult.ok) sentSmsCount++;
        }
    }

    var updatedCount = await unitOfWork.SaveChangesAsync(cancellationToken);

    return Result.Ok(new SendJobCandidateInvitationsResult
    {
        TotalTargets = candidates.Count,
        SentEmailCount = sentEmailCount,
        SentSmsCount = sentSmsCount,
        UpdatedStatusCount = updatedCount
    });
}

    private async Task<JobPointsMain?> LoadJobPointsAsync(Guid jobId)
    {
        var result = await jobPointsRepository.GetByJobIdAsync(jobId);
        return result.IsSuccess ? result.Value : null;
    }
}
