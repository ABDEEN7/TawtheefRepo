using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Events.Operation.Employee.Profile;
using Tawtheef.Domain.Utils;

namespace Tawtheef.Domain.Entities.Users;

[Table(nameof(UserProfile), Schema = Schemas.Applicant)]
public class UserProfile : EventEntity
{
    public Guid UserId { get; init; }
    public ApplicantUser? User { get; set; }
    [MaxLength(50)]
    public string Provider { get; init; } = default!;
    public Guid? CandidateTypeId { get; set; }
    public CandidateType? CandidateType { get; init; }

    public Guid? TargetEntityId { get; set; }
    public TargetEntity? TargetEntity { get; init; }

    public Guid? OfficeId { get; set; }
    public Office? Office { get; init; }

    public Guid? ResumeAttachmentId { get; set; }
    public Resource? ResumeAttachment { get; init; }

    public Guid? NationalCardId { get; set; }
    public Resource? NationalCard { get; init; }

    /// <summary>
    /// National ID number (e.g. QID)
    /// </summary>
    [MaxLength(50)]
    public string? NationalNumber { get; set; }
    public DateOnly? QIDExpiry { get; set; }

    public DateOnly? BirthDate { get; set; }

    [NotMapped]
    public int? Age
    {
        get
        {
            if (BirthDate is null) return null;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - BirthDate!.Value.Year;
            if (today < new DateOnly(today.Year, BirthDate!.Value.Month, BirthDate!.Value.Day)) age--;
            return age;
        }
    }

    public Guid? NationalityId { get; set; }
    public Country? Nationality { get; init; }

    public Guid? GenderId { get; set; }
    public Gender? Gender { get; init; }

    public Guid? ReligionId { get; set; }
    public Religion? Religion { get; init; }

    public Guid? MaritalStatusId { get; set; }
    public MaritalStatus? MaritalStatus { get; init; }

    public int ChildrenCount { get; set; }


    public Guid? ResidenceCountryId { get; set; }
    public Country? ResidenceCountry { get; init; }

    public Guid? InterviewLocationId { get; set; }
    public Country? InterviewLocation { get; init; }
    [MaxLength(500)]
    public string? Address { get; set; }

    public Guid? ResidenceAddressId { get; set; }
    public ResidenceAddress? ResidenceAddress { get; set; }

    public bool HasDisability { get; set; }
    [MaxLength(1000)]
    public string? DisabilityDetails { get; set; }

    public Guid? SponsorProfileId { get; set; }
    public SponsorProfile? SponsorProfile { get; set; }

    public Guid? BirthdayCertificateId { get; set; }
    public Resource? BirthdayCertificate { get; init; }

    public Guid? MarriageCertificateId { get; set; }
    public Resource? MarriageCertificate { get; init; }

    public ICollection<Qualification>? Qualifications { get; set; } = [];
    public ICollection<Experience>? Experiences { get; set; } = [];
    public ICollection<TrainingCourse>? TrainingCourses { get; set; } = [];
    public ICollection<Achievement>? Achievements { get; set; } = [];
    public ICollection<ProfileSkill>? Skills { get; set; } = [];
    public ICollection<ProfileLanguage>? Languages { get; set; } = [];
    public ICollection<ProfileAdditionalAttachment>? AdditionalAttachments { get; set; } = [];
    public ICollection<ProfileAssignment> ProfileAssignments { get; set; } = [];
    public ICollection<ReviewItem> ReviewItems { get; set; } = [];
    public ICollection<UserProfileLogger> UserProfileLoggers { get; set; } = [];

    public UserProfileStatus Status { get; set; } = UserProfileStatus.InCreation;

    public bool AvailableForRecruitment { get; set; } = true;

    /// <summary>
    /// Total experience years across all experiences, merging overlapping date ranges.
    /// Rounded to 1 decimal (e.g., 3.4).
    /// </summary>
    [NotMapped]
    public double CalculatedExperienceYears => ExperienceCalculator.CalculateYears(Experiences);

    public bool IsCompleted()
    {
        if (CandidateTypeId == Guid.Empty)
            return false;
        if (TargetEntityId == Guid.Empty)
            return false;
        if (ProfileValidatorUtils.RequiresOffice(CandidateTypeId, Provider) && OfficeId is null)
            return false;

        if (ProfileValidatorUtils.RequiresBirthCertificate(CandidateTypeId) && BirthdayCertificateId is null) return false;
        if (ProfileValidatorUtils.RequiresMarriageCertificate(CandidateTypeId)  && MarriageCertificateId is null) return false;
        
        if (ResumeAttachmentId is null)
            return false;
        if (NationalCardId is null)
            return false;
        
        // Required personal info
        if (string.IsNullOrWhiteSpace(NationalNumber))
            return false;
        if (NationalityId is null || NationalityId == Guid.Empty)
            return false;
        if (BirthDate is null)
            return false;
        if (GenderId is null || GenderId == Guid.Empty)
            return false;
        if (ReligionId is null || ReligionId == Guid.Empty)
            return false;
        if (MaritalStatusId is null || MaritalStatusId == Guid.Empty)
            return false;
        if (ChildrenCount < 0)
            return false;
        if (ProfileValidatorUtils.RequiresSponsor(CandidateTypeId, Provider))
        {
            if (SponsorProfileId is null) return false;
            if (SponsorProfile is null ||
                string.IsNullOrWhiteSpace(SponsorProfile.SponsorName) ||
                string.IsNullOrWhiteSpace(SponsorProfile.SponsorNumber) ||
                SponsorProfile.SponsorCardId is null)
            {
                return false;
            }
        }
        
        // Required contact info
        if (ResidenceCountryId is null || ResidenceCountryId == Guid.Empty)
            return false;
        if (InterviewLocationId is null || InterviewLocationId == Guid.Empty)
            return false;

        if (ProfileValidatorUtils.RequiresNationalAddress(CandidateTypeId, Provider))
        {
            if (ResidenceAddress is null) return false;
            if (ResidenceAddress.ZoneNo <= 0) return false;
            if (ResidenceAddress.StreetNo <= 0) return false;
            if (ResidenceAddress.BuildingNo <= 0) return false;
            if (ResidenceAddress.UnitNo < 0) return false;
            if (ResidenceAddress.CertificateId == Guid.Empty)
                return false;
        }
        else
        {
            if (Address is null) 
                return false;
        }

        if (Qualifications is null || Qualifications.Count == 0)
            return false;

        if (Languages is null || Languages.Count == 0)
            return false;

        return true;
    }

    public void FinalizeReviewProfile(bool hasCorrections)
    {
        this.Status = hasCorrections ? UserProfileStatus.RequiresUpdate: UserProfileStatus.Approved;
        AddDomainEvent(new FinalizeReviewProfileEvent(this.UserId, this.Status));
    }
}

/// <summary>
/// Utility to calculate merged experience duration.
/// Keep it in Domain/Utils (or similar) so it is testable.
/// </summary>
public static class ExperienceCalculator
{
    private const double DaysPerYear = 365.25;

    public static double CalculateYears(ICollection<Experience>? experiences)
    {
        if (experiences is null || experiences.Count == 0)
            return 0d;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Build valid ranges (inclusive day boundaries handled via day counts)
        var ranges = experiences
            .Select(exp =>
            {
                var start = exp.StartDate;
                var end = exp.EndDate ?? today;

                if (end < start)
                    return (valid: false, start: default(DateOnly), end: default(DateOnly));

                return (valid: true, start, end);
            })
            .Where(x => x.valid)
            .Select(x => (x.start, x.end))
            .OrderBy(x => x.start)
            .ToList();

        if (ranges.Count == 0)
            return 0d;

        // Merge overlaps
        var mergedStart = ranges[0].start;
        var mergedEnd = ranges[0].end;

        long totalDays = 0;

        for (var i = 1; i < ranges.Count; i++)
        {
            var current = ranges[i];

            // Overlap/adjacent check:
            // TS used: current.start <= mergedEnd (same-day overlap)
            // With DateOnly, "adjacent" (next day) is NOT overlap in your TS.
            // So we keep the same rule: current.start <= mergedEnd.
            if (current.start <= mergedEnd)
            {
                if (current.end > mergedEnd)
                    mergedEnd = current.end;
            }
            else
            {
                totalDays += DaysBetweenExclusiveEnd(mergedStart, mergedEnd);
                mergedStart = current.start;
                mergedEnd = current.end;
            }
        }

        totalDays += DaysBetweenExclusiveEnd(mergedStart, mergedEnd);

        var years = totalDays / DaysPerYear;

        // Round to 1 decimal like: Math.round(years * 10) / 10
        return Math.Round(years, 1, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Matches JS: (endMs - startMs) where both are at 00:00.
    /// That effectively counts whole days between the two dates, excluding the end date's day boundary.
    /// Example: start=2026-01-01, end=2026-01-02 => 1 day.
    /// </summary>
    private static int DaysBetweenExclusiveEnd(DateOnly start, DateOnly end)
    {
        // DateOnly.DayNumber difference matches midnight-to-midnight day boundaries.
        return end.DayNumber - start.DayNumber;
    }
}
