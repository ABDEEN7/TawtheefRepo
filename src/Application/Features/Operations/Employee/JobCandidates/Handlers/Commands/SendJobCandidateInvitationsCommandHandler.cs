using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Commands;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers.Commands;

public sealed class SendJobCandidateInvitationsCommandHandler(
    IUnitOfWork unitOfWork,
    IEmailSender emailSender,
    ISmsSender smsSender)
    : ICommandHandler<SendJobCandidateInvitationsCommand, IResult<SendJobCandidateInvitationsResult>>
{
    private readonly JobCandidatePointsCalculator _pointsCalculator = new();

    public async Task<IResult<SendJobCandidateInvitationsResult>> Handle(
        SendJobCandidateInvitationsCommand request,
        CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .Include(j => j.JobCategory)
            .Include(j => j.JobPoints).ThenInclude(p => p!.Details)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        if (job is null)
            return Result.Fail<SendJobCandidateInvitationsResult>(JobMessages.JobNotFound);

        var jobTitle = job.TitleEn;
        var targetCount = GetTargetCount(job);

        var req = await JobRequirementsService.GetAsync(unitOfWork, job, cancellationToken);

        // fully eligible base query
        var baseQuery = JobCandidatesQueryBuilder.BuildEligibleQuery(
            unitOfWork,
            job,
            req,
            request.Filter);

        // If user selected applicants explicitly -> restrict to them (but still must be eligible)
        if (request.ApplicantIds is { Count: > 0 })
        {
            baseQuery = baseQuery.Where(c => request.ApplicantIds.Contains(c.ApplicantId));
        }

        var windowSize = request.ApplicantIds is { Count: > 0 }
            ? int.MaxValue
            : Math.Max(targetCount * 10, 500);

        var window = await baseQuery
            .OrderByDescending(c => c.CreatedDate)
            .Take(windowSize)
            .ToListAsync(cancellationToken);

        if (window.Count == 0)
            return Result.Ok(EmptyResult());

        var ids = window.Select(x => x.ApplicantId).Distinct().ToList();
        var profiles = await CandidateProfileLoader.LoadForScoringAsync(unitOfWork, ids, cancellationToken);
        var profileMap = profiles.ToDictionary(p => p.UserId);

        // Score
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
            var points = _pointsCalculator.Calculate(candidate, job.JobPoints);

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
            // user-selected -> no quotas
            finalCandidates = sorted;
        }
        else
        {
            var settings = await unitOfWork.GetEntityRepository<JobCandidateFilterSetting>().DbSet
                .AsNoTracking()
                .Include(s => s.CandidateTypePercentages)
                .Include(s => s.NationalityPercentages)
                .FirstOrDefaultAsync(s => s.JobId == request.JobId, cancellationToken);

            finalCandidates = JobCandidatesFilterProcessor.ApplyPercentageFilters(sorted, settings, targetCount);
        }

        if (finalCandidates.Count == 0)
            return Result.Ok(EmptyResult());

        // Create invitations
        var invitationsRepo = unitOfWork.GetEntityRepository<Invitation>().DbSet;

        var newInvitations = finalCandidates.Select(c => new Invitation
        {
            Id = Guid.NewGuid(),
            JobId = request.JobId,
            ApplicantId = c.ApplicantId,
            InvitationStatusId = InvitationStatusIds.NewInvitation,
            CreatedDate = DateTime.UtcNow
        }).ToList();

        await invitationsRepo.AddRangeAsync(newInvitations, cancellationToken);

        // Send notifications
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
            if (!string.IsNullOrWhiteSpace(phone))
            {
                var smsResult = await smsSender.SendAsync(phone, body, cancellationToken);
                if (smsResult.ok) sentSmsCount++;
            }
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

    private static int GetTargetCount(Domain.Entities.Recruitment.Job job)
    {
        var vacancies = Math.Max(1, job.NumberOfVacancies);
        return job.JobCategoryId == JobCategoryIds.Academic ? vacancies * 5 : vacancies;
    }
}
