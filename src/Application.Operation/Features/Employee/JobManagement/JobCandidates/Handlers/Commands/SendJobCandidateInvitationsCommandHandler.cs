using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Commands;

public sealed class SendJobCandidateInvitationsCommandHandler(
    IUnitOfWork unitOfWork,
    IJobRepository jobRepository,
    IUserProfileRepository userProfileRepository,
    IJobTargetCandidateCalculatorService jobTargetCandidateCalculatorService,
    IJobRequirementsService jobRequirementsService,
    IJobCandidatesQueryBuilderService jobCandidatesQueryBuilderService,
    ILogger logger)
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
        var activeInvitationStatuses = new[]
        {
            InvitationStatusIds.NewInvitation,
            InvitationStatusIds.Read,
            InvitationStatusIds.Submitted
        };

        // Any invitation for the job blocks re-inviting (all statuses & batches).
        // If you want to block only certain statuses, add a predicate on InvitationStatusId here.
        var alreadyInvitedApplicantIds = await invitationsRepo
            .AsNoTracking()
            .Where(i => i.JobId == request.JobId && activeInvitationStatuses.AsEnumerable().Contains(i.InvitationStatusId))
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
        var profiles = await userProfileRepository.LoadForScoringAsync(ids,cancellationToken);
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
            if (job.JobPoints == null) 
                return Result.Fail<SendJobCandidateInvitationsResult>(JobMessages.JobPointsNotFound);
            var points = JobCandidatePointsCalculator.Calculate(candidate, job.JobPoints,job.JobDegrees,job.MajorId,job.SubMajorId,logger);

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
            finalCandidates = sorted.Take(targetCount).ToList();
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

        var newInvitations = new List<Invitation>();
        foreach (var candidate in finalCandidates)
        {
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                JobId = request.JobId,
                ApplicantId = candidate.ApplicantId,
                InvitationStatusId = InvitationStatusIds.NewInvitation,
                BatchNumber = batchNumber
            };

            invitation.AddDomainEvent(new JobCandidateInvitationSentDomainEvent(
                invitation.Id,
                candidate.ApplicantId,
                candidate.Applicant?.Email,
                candidate.Applicant?.PhoneNumber,
                jobTitle ?? string.Empty,
                DateTimeOffset.UtcNow));

            newInvitations.Add(invitation);
        }

        await invitationsRepo.AddRangeAsync(newInvitations, cancellationToken);

        var sentEmailCount = finalCandidates.Count(c => !string.IsNullOrWhiteSpace(c.Applicant?.Email));
        var sentSmsCount = finalCandidates.Count(c => !string.IsNullOrWhiteSpace(c.Applicant?.PhoneNumber));

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
