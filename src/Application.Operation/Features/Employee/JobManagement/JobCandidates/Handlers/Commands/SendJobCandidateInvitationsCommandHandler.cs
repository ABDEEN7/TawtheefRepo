using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
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
    IJobTargetCandidateCalculatorService targetCalculator,
    IJobRequirementsService jobRequirementsService,
    IJobCandidatesQueryBuilderService queryBuilder,
    IAppLogger logger)
    : IRequestHandler<SendJobCandidateInvitationsCommand, IResult<SendJobCandidateInvitationsResult>>
{
    public async Task<IResult<SendJobCandidateInvitationsResult>> Handle(
        SendJobCandidateInvitationsCommand request,
        CancellationToken cancellationToken)
    {
        var job = await jobRepository.LoadJobWithPointsAsync(request.JobId);
        if (job is null)
            return Result.Fail<SendJobCandidateInvitationsResult>(JobMessages.JobNotFound);

        if (job.JobPoints is null)
            return Result.Fail<SendJobCandidateInvitationsResult>(JobMessages.JobPointsNotFound);

        var jobTitle = job.JobTitle?.JobNameEn ?? job.JobTitle?.JobNameAr ?? string.Empty;

        var targetCount = await targetCalculator
            .GetTargetCountAsync(job.JobCategoryId, job.NumberOfVacancies);

        var requirements = await jobRequirementsService
            .GetAsync(job);

        var eligibleQuery = BuildEligibleCandidatesQuery(job, request, requirements);

        var alreadyInvited = await GetAlreadyInvitedApplicants(request.JobId, cancellationToken);

        var candidateWindow = await LoadCandidateWindow(
            eligibleQuery,
            request,
            targetCount,
            alreadyInvited,
            cancellationToken);

        if (candidateWindow.Count == 0)
            return Result.Ok(EmptyResult());

        var scoredCandidates = await ScoreCandidates(
            candidateWindow,
            job,
            request.Filter?.MinimumPoints,
            cancellationToken);

        if (scoredCandidates.Count == 0)
            return Result.Ok(EmptyResult());

        var finalCandidates = await SelectFinalCandidates(
            scoredCandidates,
            request,
            targetCount,
            cancellationToken);

        if (finalCandidates.Count == 0)
            return Result.Ok(EmptyResult());

        return await CreateInvitations(
            finalCandidates,
            jobTitle,
            request.JobId,
            cancellationToken);
    }

    private IQueryable<JobCandidateRecord> BuildEligibleCandidatesQuery(
        Tawtheef.Domain.Entities.Recruitment.Job job,
        SendJobCandidateInvitationsCommand request,
        JobRequirements req)
    {
        var query = queryBuilder.BuildEligibleQuery(
            job.Id,
            job.WorkLocationId,
            job.GenderId,
            job.MaximumAge,
            job.MinimumAge,
            req,
            request.Filter);

        if (request.ApplicantIds is { Count: > 0 })
            query = query.Where(c => request.ApplicantIds.Contains(c.ApplicantId));

        return query;
    }

    private async Task<HashSet<Guid>> GetAlreadyInvitedApplicants(
        Guid jobId,
        CancellationToken ct)
    {
        var repo = unitOfWork.GetEntityRepository<Invitation>().DbSet;

        var activeStatuses = new[]
        {
            InvitationStatusIds.NewInvitation,
            InvitationStatusIds.Read,
            InvitationStatusIds.ExamEligible
        };

        var ids = await repo
            .AsNoTracking()
            .Where(i => i.JobId == jobId && activeStatuses.Contains(i.InvitationStatusId))
            .Select(i => i.ApplicantId)
            .Distinct()
            .ToListAsync(ct);

        return ids.ToHashSet();
    }

    private async Task<List<JobCandidateRecord>> LoadCandidateWindow(
        IQueryable<JobCandidateRecord> query,
        SendJobCandidateInvitationsCommand request,
        int targetCount,
        HashSet<Guid> alreadyInvited,
        CancellationToken ct)
    {
        var windowSize = request.ApplicantIds is { Count: > 0 }
            ? int.MaxValue
            : Math.Max(targetCount * 10, 500);

        var window = await query
            .OrderByDescending(c => c.CreatedDate)
            .Take(windowSize)
            .ToListAsync(ct);

        return window
            .Where(c => !alreadyInvited.Contains(c.ApplicantId))
            .ToList();
    }

    private async Task<List<JobCandidateRecord>> ScoreCandidates(
        List<JobCandidateRecord> window,
        Tawtheef.Domain.Entities.Recruitment.Job job,
        int? minimumPoints,
        CancellationToken ct)
    {
        var applicantIds = window.Select(c => c.ApplicantId).Distinct().ToList();

        var profiles = await userProfileRepository
            .LoadForScoringAsync(applicantIds, ct);

        var profileMap = profiles.ToDictionary(p => p.UserId);

        var scored = new List<JobCandidateRecord>(window.Count);

        foreach (var candidate in window)
        {
            if (!profileMap.TryGetValue(candidate.ApplicantId, out var profile))
                continue;

            var major = profile.Qualifications?
                .OrderByDescending(q => q.GraduationYear)
                .Select(q => q.Major)
                .FirstOrDefault();

            var enriched = candidate with
            {
                Applicant = profile.User,
                Profile = profile,
                Major = major
            };

            var points = JobCandidatePointsCalculator.Calculate(
                enriched,
                job.JobPoints!,
                job.JobDegrees,
                job.MajorId,
                job.SubMajorId,
                logger);

            var scoredCandidate = enriched with { Points = points };

            if (minimumPoints is null || scoredCandidate.Points >= minimumPoints)
                scored.Add(scoredCandidate);
        }

        return scored
            .OrderByDescending(c => c.Points)
            .ThenByDescending(c => c.CreatedDate)
            .ToList();
    }

    private async Task<List<JobCandidateRecord>> SelectFinalCandidates(
        List<JobCandidateRecord> sorted,
        SendJobCandidateInvitationsCommand request,
        int targetCount,
        CancellationToken ct)
    {
        if (request.ApplicantIds is { Count: > 0 })
            return sorted.Take(targetCount).ToList();

        var settings = await unitOfWork
            .GetEntityRepository<JobCandidateFilterSetting>()
            .DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(s => s.CandidateTypePercentages)
            .Include(s => s.NationalityPercentages)
            .FirstOrDefaultAsync(s => s.JobId == request.JobId, ct);

        return JobCandidatesFilterUtility
            .ApplyPercentageFilters(sorted, settings, targetCount);
    }

    private async Task<IResult<SendJobCandidateInvitationsResult>> CreateInvitations(
        List<JobCandidateRecord> candidates,
        string jobTitle,
        Guid jobId,
        CancellationToken ct)
    {
        var batch = Guid.NewGuid();

        var repo = unitOfWork.GetEntityRepository<Invitation>().DbSet;

        var invitations = candidates.Select(candidate =>
        {
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                JobId = jobId,
                ApplicantId = candidate.ApplicantId,
                InvitationStatusId = InvitationStatusIds.NewInvitation,
                BatchNumber = batch
            };

            invitation.AddDomainEvent(new JobCandidateInvitationSentDomainEvent(
                invitation.Id,
                candidate.ApplicantId,
                candidate.Applicant?.Email,
                candidate.Applicant?.PhoneNumber,
                jobTitle,
                DateTimeOffset.UtcNow));

            return invitation;
        }).ToList();

        await repo.AddRangeAsync(invitations, ct);

        var saved = await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok(new SendJobCandidateInvitationsResult
        {
            TotalTargets = candidates.Count,
            SentEmailCount = candidates.Count(c => !string.IsNullOrWhiteSpace(c.Applicant?.Email)),
            SentSmsCount = candidates.Count(c => !string.IsNullOrWhiteSpace(c.Applicant?.PhoneNumber)),
            UpdatedStatusCount = saved
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
