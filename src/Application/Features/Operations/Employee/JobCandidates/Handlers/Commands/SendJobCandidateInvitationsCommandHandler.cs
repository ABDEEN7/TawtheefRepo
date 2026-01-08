using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services.Interfaces;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Utilities;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Commands;

public sealed class SendJobCandidateInvitationsCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    IJobRepository jobRepository,
    IUserProfileRepository userProfileRepository,
    IJobTargetCandidateCalculatorService jobTargetCandidateCalculatorService,
    IJobRequirementsService jobRequirementsService,
    IJobCandidatesQueryBuilderService jobCandidatesQueryBuilderService,
    ISmsSender smsSender)
    : ICommandHandler<SendJobCandidateInvitationsCommand, IResult<SendJobCandidateInvitationsResult>>
{
    public async Task<IResult<SendJobCandidateInvitationsResult>> Handle(
        SendJobCandidateInvitationsCommand request,
        CancellationToken cancellationToken)
    {
        var job = await jobRepository.LoadJobWithPointsAsync(request.JobId);
        if (job is null)
            return Result.Fail<SendJobCandidateInvitationsResult>(JobMessages.JobNotFound);

        var jobTitle = job.TitleEn;
        var targetCount = await jobTargetCandidateCalculatorService
            .GetTargetCountAsync(job.JobCategoryId, job.NumberOfVacancies);

        var req = await jobRequirementsService.GetAsync(job.MajorId, job.SubMajorId);

        var baseQuery = jobCandidatesQueryBuilderService.BuildEligibleQuery(
            job.Id, job.GenderId, job.MaximumAge, job.MinimumAge, req, request.Filter);

        if (request.ApplicantIds is { Count: > 0 })
        {
            baseQuery = baseQuery.Where(c => request.ApplicantIds.Contains(c.ApplicantId));
        }

        // --------------------------------------------------------------------
        // NEW: Do not invite again (exclude applicants who already have invitations for this job)
        // --------------------------------------------------------------------
        var invitationsRepo = unitOfWork.GetEntityRepository<Invitation>().DbSet;

        // Any invitation for the job blocks re-inviting (all statuses & batches).
        // If you want to block only certain statuses, add a predicate on InvitationStatusId here.
        var alreadyInvitedApplicantIds = await invitationsRepo
            .AsNoTracking()
            .Where(i => i.JobId == request.JobId)
            .Select(i => i.ApplicantId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var alreadyInvitedSet = alreadyInvitedApplicantIds.ToHashSet();
        // --------------------------------------------------------------------

        var windowSize = request.ApplicantIds is { Count: > 0 }
            ? int.MaxValue
            : Math.Max(targetCount * 10, 500);

        var window = await baseQuery
            .OrderByDescending(c => c.CreatedDate)
            .Take(windowSize)
            .ToListAsync(cancellationToken);

        if (window.Count == 0)
            return Result.Ok(EmptyResult());

        // NEW: Filter out already invited candidates early (before scoring)
        window = window
            .Where(c => !alreadyInvitedSet.Contains(c.ApplicantId))
            .ToList();

        if (window.Count == 0)
            return Result.Ok(EmptyResult());

        var ids = window.Select(x => x.ApplicantId).Distinct().ToList();
        var profiles = await userProfileRepository.LoadForScoringAsync(ids);
        var profileMap = profiles.ToDictionary(p => p.UserId);

        var scored = new List<JobCandidateRecord>(window.Count);
        foreach (var c in window)
        {
            if (!profileMap.TryGetValue(c.ApplicantId, out var p))
                continue;

            var major = p.Qualifications?
                .OrderByDescending(q => q.GraduationYear)
                .Select(q => q.Major)
                .FirstOrDefault();

            var candidate = c with { Applicant = p.User, Profile = p, Major = major };
            var points = JobCandidatePointsCalculator.Calculate(candidate, job.JobPoints);

            scored.Add(candidate with { Points = points });
        }

        if (request.Filter?.MinimumPoints is { } minPoints)
            scored = scored.Where(c => c.Points >= minPoints).ToList();

        if (scored.Count == 0)
            return Result.Ok(EmptyResult());

        var sorted = scored
            .OrderByDescending(c => c.Points)
            .ThenByDescending(c => c.CreatedDate)
            .ToList();

        List<JobCandidateRecord> finalCandidates;

        if (request.ApplicantIds is { Count: > 0 })
        {
            finalCandidates = sorted;
        }
        else
        {
            var settings = await unitOfWork.GetEntityRepository<JobCandidateFilterSetting>().DbSet
                .AsNoTracking()
                .Include(s => s.CandidateTypePercentages)
                .Include(s => s.NationalityPercentages)
                .FirstOrDefaultAsync(s => s.JobId == request.JobId, cancellationToken);

            finalCandidates = JobCandidatesFilterUtility.ApplyPercentageFilters(sorted, settings, targetCount);
        }

        if (finalCandidates.Count == 0)
            return Result.Ok(EmptyResult());

        // NEW: Safety filter (in case any slip through)
        finalCandidates = finalCandidates
            .Where(c => !alreadyInvitedSet.Contains(c.ApplicantId))
            .ToList();

        if (finalCandidates.Count == 0)
            return Result.Ok(EmptyResult());

        // Keep batch numbering logic
        var batchNumber = Guid.NewGuid();

        var newInvitations = finalCandidates.Select(c => new Invitation
        {
            Id = Guid.NewGuid(),
            JobId = request.JobId,
            ApplicantId = c.ApplicantId,
            InvitationStatusId = InvitationStatusIds.NewInvitation,
            BatchNumber = batchNumber,
            CreatedDate = DateTime.UtcNow
        }).ToList();

        await invitationsRepo.AddRangeAsync(newInvitations, cancellationToken);

        var sentEmailCount = 0;
        var sentSmsCount = 0;

        foreach (var candidate in finalCandidates)
        {
            var body = string.IsNullOrWhiteSpace(jobTitle)
                ? "You have been invited to apply for a job on Tawtheef."
                : $"You have been invited to apply for {jobTitle} on Tawtheef.";

            var email = candidate.Applicant?.Email;
            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailResult = await emailSender.SendAsync(email, "Job Invitation", body, cancellationToken);
                if (emailResult.ok) sentEmailCount++;
            }

            var phone = candidate.Applicant?.PhoneNumber;
            if (string.IsNullOrWhiteSpace(phone))
                continue;

            var smsResult = await smsSender.SendAsync(phone, body, cancellationToken);
            if (smsResult.ok) sentSmsCount++;
        }

        var updatedCount = await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(new SendJobCandidateInvitationsResult
        {
            TotalTargets = finalCandidates.Count,
            SentEmailCount = sentEmailCount,
            SentSmsCount = sentSmsCount,
            UpdatedStatusCount = updatedCount
        });
    }

    private static SendJobCandidateInvitationsResult EmptyResult() => new()
    {
        TotalTargets = 0,
        SentEmailCount = 0,
        SentSmsCount = 0,
        UpdatedStatusCount = 0
    };
}
