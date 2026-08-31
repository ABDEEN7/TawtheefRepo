using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;
using FluentResults;
using MediatR;
using Microsoft.Data.SqlClient;
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
    IInvitationExpiryConfigurationRepository invitationExpiryConfigurationRepository,
    IJobTargetCandidateCalculatorService targetCalculator,
    IJobRequirementsService jobRequirementsService,
    IJobCandidatesQueryBuilderService queryBuilder,
    IAppLogger logger)
    : IRequestHandler<SendJobCandidateInvitationsCommand, IResult<SendJobCandidateInvitationsResult>>
{
    private const string ActiveInvitationUniqueIndexName =
        "IX_Invitation_ApplicantId_JobId_Active";

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
        var activeInvitationCount = await CountActiveInvitations(request.JobId, cancellationToken);
        var availableVacancies = Math.Max(targetCount - activeInvitationCount, 0);

        if (availableVacancies == 0)
            return Result.Fail<SendJobCandidateInvitationsResult>(
                JobCandidatesMessages.JobCandidateInvitationVacanciesFull);

        var requirements = await jobRequirementsService
            .GetAsync(job);

        var eligibleQuery = BuildEligibleCandidatesQuery(job, request, requirements);

        var alreadyInvited = await GetAlreadyInvitedApplicants(request.JobId, cancellationToken);

        var candidateWindow = await LoadCandidateWindow(
            eligibleQuery,
            request,
            availableVacancies,
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
            availableVacancies,
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
        Job job,
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

        var ids = await repo
            .AsNoTracking()
            .Where(i => i.JobId == jobId && CandidateEligibilityRules.ActiveInvitationStatuses.Contains(i.InvitationStatusId))
            .Select(i => i.ApplicantId)
            .Distinct()
            .ToListAsync(ct);

        return ids.ToHashSet();
    }

    private async Task<int> CountActiveInvitations(
        Guid jobId,
        CancellationToken ct)
    {
        var repo = unitOfWork.GetEntityRepository<Invitation>().DbSet;

        return await repo
            .AsNoTracking()
            .CountAsync(i => i.JobId == jobId && CandidateEligibilityRules.ActiveInvitationStatuses.Contains(i.InvitationStatusId), ct);
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
        List<JobCandidateRecord> window,Job job,int? minimumPoints,CancellationToken ct)
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
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        var expiryDays = await GetInvitationExpiryDaysAsync();
        var expiresOn = currentDate.AddDays(expiryDays + 1);

        var repo = unitOfWork.GetEntityRepository<Invitation>().DbSet;

        var invitations = candidates.Select(candidate =>
        {
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                JobId = jobId,
                ApplicantId = candidate.ApplicantId,
                InvitationStatusId = InvitationStatusIds.NewInvitation,
                BatchNumber = batch,
                ExpiresOn = expiresOn
            };

            invitation.AddDomainEvent(new JobCandidateInvitationSentDomainEvent(
                invitation.Id,
                candidate.ApplicantId,
                candidate.Applicant?.Email,
                candidate.Applicant?.PhoneNumber,
                jobTitle,
                expiryDays,
                expiresOn,
                DateTimeOffset.UtcNow));

            return invitation;
        }).ToList();

        await repo.AddRangeAsync(invitations, ct);

        int saved;
        try
        {
            saved = await unitOfWork.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (IsActiveInvitationDuplicate(exception))
        {
            return Failure(JobCandidatesMessages.JobCandidateInvitationConflict, 409);
        }

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

    private static bool IsActiveInvitationDuplicate(DbUpdateException exception)
    {
        return exception.InnerException is SqlException sqlException &&
               sqlException.Errors.Cast<SqlError>().Any(error =>
                   (error.Number is 2601 or 2627) &&
                   error.Message.Contains(ActiveInvitationUniqueIndexName, StringComparison.OrdinalIgnoreCase));
    }

    private static Result<SendJobCandidateInvitationsResult> Failure(string code, int statusCode) =>
        Result.Fail<SendJobCandidateInvitationsResult>(
            new Error(code)
                .WithMetadata("Code", code)
                .WithMetadata("UserMessage", code)
                .WithMetadata("StatusCode", statusCode));

    private async Task<int> GetInvitationExpiryDaysAsync()
    {
        var configuration = await invitationExpiryConfigurationRepository.GetAsync();
        return configuration.IsSuccess ? configuration.Value.ExpiryDays : 7;
    }
}
