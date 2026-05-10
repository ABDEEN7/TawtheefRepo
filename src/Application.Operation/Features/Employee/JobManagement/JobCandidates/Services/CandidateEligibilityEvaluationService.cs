using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Utilities;
using Application.Operation.Features.Employee.ProfileManagement;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;

public class CandidateEligibilityEvaluationService(
    IUnitOfWork unitOfWork,
    IJobRepository jobRepository,
    IJobRequirementsService jobRequirementsService,
    IJobCandidatesQueryBuilderService jobCandidatesQueryBuilderService,
    IUnitOfWork uow,
    IAppLogger logger)
    : ICandidateEligibilityEvaluationService
{
    public async Task<Result<CandidateEligibilityResultDto>> EvaluateAsync(
        Guid jobId, Guid candidateId, string language, CancellationToken cancellationToken)
    {
        var job = await jobRepository.LoadJobWithPointsAsync(jobId);

        if (job is null)
        {
            return Result.Fail<CandidateEligibilityResultDto>(
                CandidateEligibilityErrorCodes.JobNotFound);
        }

        var candidate = await UserProfileQueryFactory.CreateFullQuery(uow, false)
            .FirstOrDefaultAsync(p => p.UserId == candidateId, cancellationToken);
        if (candidate is null)
        {
            return Result.Fail<CandidateEligibilityResultDto>(
                CandidateEligibilityErrorCodes.CandidateNotFound);
        }

        var requirements = await jobRequirementsService.GetAsync(job);

        var invitations = await GetInvitationsForJob(
            jobId,
            candidateId,
            cancellationToken);

        var conditions = EvaluateConditions(
            job,
            candidate,
            requirements,
            invitations,
            language);

        var scoredCandidates = JobCandidateScoringUtility.Score(
            [new JobCandidateRecord
            {
                ApplicantId = candidateId,
                Applicant = candidate.User,
                Profile = candidate,
                JobId = jobId,
                Job = job
            }],
            [candidate],
            job.JobPoints!,
            job.MajorId,
            job.SubMajorId,
            job.JobDegrees,
            logger);

        var points = scoredCandidates.First().PointsBreakdown!;

        var result = new CandidateEligibilityResultDto
        {
            JobId = jobId,
            CandidateId = candidateId,
            CandidateName = candidate.User?.FullNameAr
                ?? candidate.User?.FullNameEn
                ?? CandidateEligibilityValueCodes.Unknown,
            NationalNumber = candidate.NationalNumber ?? CandidateEligibilityValueCodes.Unknown,

            Profile = new CandidateEligibilityProfileSummaryDto
            {
                Status = candidate.Status.ToString(),
                AvailableForRecruitment = candidate.AvailableForRecruitment,
                TargetEntityId = candidate.TargetEntityId,
                GenderId = candidate.GenderId,
                BirthDate = candidate.BirthDate,
                QualificationLevelIds = candidate.Qualifications?.Select(x => x.DegreeId).ToList() ?? [],
                SkillIds = candidate.Skills?.Select(x => x.SkillId).ToList() ?? []
            },

            JobRequirements = new JobEligibilityRequirementSummaryDto
            {
                TargetEntityId = job.WorkLocationId,
                GenderId = job.GenderId,
                MaximumAge = job.MaximumAge,
                MinimumAge = job.MinimumAge,
                QualificationLevelIds = requirements.QualificationLevelIds.ToList(),
                RequiredSkillIds = requirements.RequiredSkillIds.ToList()
            },

            Conditions = conditions,

            IsEligible = conditions.All(c =>
                c.Status is CandidateEligibilityStatuss.Passed
                    or CandidateEligibilityStatuss.NotApplicable),

            PointsBreakdown = new CandidatePointsBreakdownDto
            {
                CategoryPoints = points.CategoryPoints,
                EducationPoints = points.EducationPoints,
                ExperiencePoints = points.ExperiencePoints,
                TrainingPoints = points.TrainingPoints,
                SkillPoints = points.SkillPoints,
                LanguagePoints = points.LanguagePoints,
                CertificatePoints = points.CertificatePoints,
                TotalPoints = points.TotalPoints,
                Details = points.Details
            },

            AppearsInEligibleCandidatesQuery = await jobCandidatesQueryBuilderService
                .BuildEligibleQuery(
                    job.Id,
                    job.WorkLocationId,
                    job.GenderId,
                    job.MaximumAge,
                    job.MinimumAge,
                    requirements,
                    null)
                .AnyAsync(x => x.ApplicantId == candidateId, cancellationToken)
        };

        return Result.Ok(result);
    }


    private async Task<List<Invitation>> GetInvitationsForJob(
        Guid jobId,
        Guid candidateId,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(i => i.JobId == jobId && i.ApplicantId == candidateId)
            .ToListAsync(cancellationToken);
    }

    private static List<CandidateEligibilityConditionDto> EvaluateConditions(
    Job job,
    UserProfile candidate,
    JobRequirements requirements,
    List<Invitation> invitations,
    string language)
{
    var candidateQualificationIds = candidate.Qualifications?
        .Select(q => q.DegreeId)
        .Distinct()
        .ToList() ?? [];

    var candidateSkillIds = candidate.Skills?
        .Select(s => s.SkillId)
        .Distinct()
        .ToList() ?? [];

    var hasActiveInvitation = invitations.Any(i =>
        CandidateEligibilityRules.ActiveInvitationStatuses.Contains(i.InvitationStatusId));

    var conditions = new List<CandidateEligibilityConditionDto>
    {
        BuildCondition(
            CandidateEligibilityConditionCodes.ProfileApproved,
            CandidateEligibilityTranslationKeys.Conditions.ProfileApproved.Label,
            candidate.Status == UserProfileStatus.Approved,
            CandidateEligibilityValueCodes.Approved,
            candidate.Status == UserProfileStatus.Approved
                ? CandidateEligibilityValueCodes.Approved
                : CandidateEligibilityValueCodes.NotApproved,
            CandidateEligibilityTranslationKeys.Conditions.ProfileApproved.FailureReason),

        BuildCondition(
            CandidateEligibilityConditionCodes.AvailableForRecruitment,
            CandidateEligibilityTranslationKeys.Conditions.AvailableForRecruitment.Label,
            candidate.AvailableForRecruitment,
            CandidateEligibilityValueCodes.Available,
            candidate.AvailableForRecruitment
                ? CandidateEligibilityValueCodes.Available
                : CandidateEligibilityValueCodes.NotAvailable,
            CandidateEligibilityTranslationKeys.Conditions.AvailableForRecruitment.FailureReason),

        BuildCondition(
            CandidateEligibilityConditionCodes.TargetEntityMatch,
            CandidateEligibilityTranslationKeys.Conditions.TargetEntityMatch.Label,
            candidate.TargetEntityId == job.WorkLocationId,
            FormatLocalizedName(job.WorkLocation, language),
            FormatLocalizedName(candidate.TargetEntity, language),
            CandidateEligibilityTranslationKeys.Conditions.TargetEntityMatch.FailureReason),

        BuildCondition(
            CandidateEligibilityConditionCodes.NoActiveInvitation,
            CandidateEligibilityTranslationKeys.Conditions.NoActiveInvitation.Label,
            !hasActiveInvitation,
            CandidateEligibilityValueCodes.NoActiveInvitation,
            hasActiveInvitation
                ? CandidateEligibilityValueCodes.HasActiveInvitation
                : CandidateEligibilityValueCodes.NoActiveInvitation,
            CandidateEligibilityTranslationKeys.Conditions.NoActiveInvitation.FailureReason),

        BuildCondition(
            CandidateEligibilityConditionCodes.GenderMatch,
            CandidateEligibilityTranslationKeys.Conditions.GenderMatch.Label,
            IsGenderMatching(job, candidate),
            job.GenderId is null || job.GenderId == GenderIds.All
                ? CandidateEligibilityValueCodes.All
                : FormatLocalizedName(job.Gender, language),
            FormatLocalizedName(candidate.Gender, language),
            CandidateEligibilityTranslationKeys.Conditions.GenderMatch.FailureReason),

        BuildCondition(
            CandidateEligibilityConditionCodes.BirthDateExists,
            CandidateEligibilityTranslationKeys.Conditions.BirthDateExists.Label,
            candidate.BirthDate.HasValue,
            CandidateEligibilityValueCodes.BirthDateExists,
            candidate.BirthDate?.ToString("yyyy-MM-dd") ?? CandidateEligibilityValueCodes.Missing,
            CandidateEligibilityTranslationKeys.Conditions.BirthDateExists.FailureReason),

        BuildAgeCondition(job, candidate),

        BuildCondition(
            CandidateEligibilityConditionCodes.QualificationMatch,
            CandidateEligibilityTranslationKeys.Conditions.QualificationMatch.Label,
            requirements.QualificationLevelIds.Count == 0 ||
                candidateQualificationIds.Any(requirements.QualificationLevelIds.Contains),
            FormatLocalizedNames(job.JobDegrees?.Select(d => d.Degree), language),
            FormatLocalizedNames(candidate.Qualifications?.Select(q => q.Degree), language),
            CandidateEligibilityTranslationKeys.Conditions.QualificationMatch.FailureReason),

        BuildSkillsCondition(job, candidate, requirements, candidateSkillIds, language)
    };

    return conditions;
}

private static CandidateEligibilityConditionDto BuildCondition(
    string code,
    string label,
    bool passed,
    string expectedValue,
    string actualValue,
    string failureReason)
{
    return new CandidateEligibilityConditionDto
    {
        Code = code,
        Label = label,
        Status = passed
            ? CandidateEligibilityStatuss.Passed
            : CandidateEligibilityStatuss.Failed,
        ExpectedValue = expectedValue,
        ActualValue = actualValue,
        FailureReason = passed ? null : failureReason
    };
}

private static CandidateEligibilityConditionDto BuildAgeCondition(
    Job job,
    UserProfile candidate)
{
    if (!candidate.BirthDate.HasValue)
    {
        return new CandidateEligibilityConditionDto
        {
            Code = CandidateEligibilityConditionCodes.AgeWithinRange,
            Label = CandidateEligibilityTranslationKeys.Conditions.AgeWithinRange.Label,
            Status = CandidateEligibilityStatuss.NotApplicable,
            ExpectedValue = CandidateEligibilityTranslationKeys.Values.AgeRange,
            ExpectedValueParams = new
            {
                min = job.MinimumAge,
                max = job.MaximumAge
            },
            ActualValue = CandidateEligibilityValueCodes.MissingBirthDate,
            FailureReason = CandidateEligibilityTranslationKeys.Conditions.AgeWithinRange.MissingBirthDateReason
        };
    }

    var age = CalculateAge(candidate.BirthDate.Value);
    var passed = CandidateEligibilityRules.IsAgeWithinRange(
        candidate.BirthDate.Value,
        job.MinimumAge,
        job.MaximumAge);

    return new CandidateEligibilityConditionDto
    {
        Code = CandidateEligibilityConditionCodes.AgeWithinRange,
        Label = CandidateEligibilityTranslationKeys.Conditions.AgeWithinRange.Label,
        Status = passed
            ? CandidateEligibilityStatuss.Passed
            : CandidateEligibilityStatuss.Failed,
        ExpectedValue = CandidateEligibilityTranslationKeys.Values.AgeRange,
        ExpectedValueParams = new
        {
            min = job.MinimumAge,
            max = job.MaximumAge
        },
        ActualValue = CandidateEligibilityTranslationKeys.Values.Age,
        ActualValueParams = new
        {
            age
        },
        FailureReason = passed
            ? null
            : CandidateEligibilityTranslationKeys.Conditions.AgeWithinRange.FailureReason,
        FailureReasonParams = passed
            ? null
            : new
            {
                age
            }
    };
}

private static CandidateEligibilityConditionDto BuildSkillsCondition(
    Job job,
    UserProfile candidate,
    JobRequirements requirements,
    List<Guid> candidateSkillIds,
    string language)
{
    var passed = CandidateEligibilityRules.HasAllRequiredSkills(
        candidateSkillIds,
        requirements.RequiredSkillIds);

    var missingSkillIds = requirements.RequiredSkillIds
        .Where(id => !candidateSkillIds.Contains(id))
        .ToList();

    var missingNames = FormatLocalizedNames(
        job.JobSkills?
            .Where(js => missingSkillIds.Contains(js.SkillId))
            .Select(js => js.Skill),
        language);

    return new CandidateEligibilityConditionDto
    {
        Code = CandidateEligibilityConditionCodes.RequiredSkillsMatch,
        Label = CandidateEligibilityTranslationKeys.Conditions.RequiredSkillsMatch.Label,
        Status = passed
            ? CandidateEligibilityStatuss.Passed
            : CandidateEligibilityStatuss.Failed,
        ExpectedValue = FormatLocalizedNames(job.JobSkills?.Select(s => s.Skill), language),
        ActualValue = FormatLocalizedNames(candidate.Skills?.Select(s => s.Skill), language),
        FailureReason = passed
            ? null
            : CandidateEligibilityTranslationKeys.Conditions.RequiredSkillsMatch.FailureReason,
        FailureReasonParams = passed
            ? null
            : new
            {
                missing = missingNames
            }
    };
}

private static bool IsGenderMatching(Job job, UserProfile candidate)
{
    return job.GenderId is null ||
           job.GenderId == GenderIds.All ||
           candidate.GenderId == job.GenderId;
}


    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
    private static string FormatLocalizedName(
        Tawtheef.Domain.Common.Interfaces.ILocalizedName? entity,
        string language)
    {
        if (entity is null)
        {
            return string.Empty;
        }

        return IsArabic(language) ? entity.NameAr : entity.NameEn;
    }

    private static string FormatLocalizedNames<T>(
        IEnumerable<T?>? entities,
        string language)
        where T : Tawtheef.Domain.Common.Interfaces.ILocalizedName
    {
        var names = entities?
            .Where(e => e is not null)
            .Select(e => FormatLocalizedName(e, language))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        return names is { Count: > 0 }
            ? string.Join(", ", names)
            : CandidateEligibilityValueCodes.None;
    }

    private static bool IsArabic(string language)
    {
        return string.Equals(language, "ar", StringComparison.OrdinalIgnoreCase);
    }
}

public static class CandidateEligibilityTranslationKeys
{
    public static class Conditions
    {
        public static class ProfileApproved
        {
            public const string Label = "candidateEligibility.conditions.profileApproved.label";
            public const string FailureReason = "candidateEligibility.conditions.profileApproved.failureReason";
        }

        public static class AvailableForRecruitment
        {
            public const string Label = "candidateEligibility.conditions.availableForRecruitment.label";
            public const string FailureReason = "candidateEligibility.conditions.availableForRecruitment.failureReason";
        }

        public static class TargetEntityMatch
        {
            public const string Label = "candidateEligibility.conditions.targetEntityMatch.label";
            public const string FailureReason = "candidateEligibility.conditions.targetEntityMatch.failureReason";
        }

        public static class NoActiveInvitation
        {
            public const string Label = "candidateEligibility.conditions.noActiveInvitation.label";
            public const string FailureReason = "candidateEligibility.conditions.noActiveInvitation.failureReason";
        }

        public static class GenderMatch
        {
            public const string Label = "candidateEligibility.conditions.genderMatch.label";
            public const string FailureReason = "candidateEligibility.conditions.genderMatch.failureReason";
        }

        public static class BirthDateExists
        {
            public const string Label = "candidateEligibility.conditions.birthDateExists.label";
            public const string FailureReason = "candidateEligibility.conditions.birthDateExists.failureReason";
        }

        public static class AgeWithinRange
        {
            public const string Label = "candidateEligibility.conditions.ageWithinRange.label";
            public const string FailureReason = "candidateEligibility.conditions.ageWithinRange.failureReason";

            public const string MissingBirthDateReason =
                "candidateEligibility.conditions.ageWithinRange.missingBirthDateReason";
        }

        public static class QualificationMatch
        {
            public const string Label = "candidateEligibility.conditions.qualificationMatch.label";
            public const string FailureReason = "candidateEligibility.conditions.qualificationMatch.failureReason";
        }

        public static class RequiredSkillsMatch
        {
            public const string Label = "candidateEligibility.conditions.requiredSkillsMatch.label";
            public const string FailureReason = "candidateEligibility.conditions.requiredSkillsMatch.failureReason";
        }
    }

    public static class Values
    {
        public const string AgeRange = "candidateEligibility.values.ageRange";
        public const string Age = "candidateEligibility.values.age";
        public const string QualificationIds = "candidateEligibility.values.qualificationIds";
        public const string SkillIds = "candidateEligibility.values.skillIds";
    }
}
