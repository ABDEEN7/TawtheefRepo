using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Utils;

namespace Tawtheef.Domain.Entities.Users;

[Table(nameof(UserProfile), Schema = Schemas.Applicant)]
public class UserProfile : EventEntity
{
    public Guid UserId { get; set; }
    public ApplicantUser? User { get; set; }

    public string Provider { get; set; } = default!;
    public Guid CandidateTypeId { get; set; }
    public CandidateType? CandidateType { get; set; }

    public Guid TargetEntityId { get; set; }
    public TargetEntity? TargetEntity { get; set; }

    public Guid? OfficeId { get; set; }
    public Office? Office { get; set; }

    public Guid? ResumeAttachmentId { get; set; }
    public Resource? ResumeAttachment { get; set; }

    public Guid? NationalCardId { get; set; }
    public Resource? NationalCard { get; set; }

    /// <summary>
    /// National ID number (e.g. QID)
    /// </summary>
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
    public Country? Nationality { get; set; }

    public Guid? GenderId { get; set; }
    public Gender? Gender { get; init; }

    public Guid? ReligionId { get; set; }
    public Religion? Religion { get; init; }

    public Guid? MaritalStatusId { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }

    public int ChildrenCount { get; set; }


    public Guid? ResidenceCountryId { get; set; }
    public Country? ResidenceCountry { get; set; }

    public Guid? InterviewLocationId { get; set; }
    public Country? InterviewLocation { get; set; }

    public string? Address { get; set; }

    public Guid? ResidenceAddressId { get; set; }
    public ResidenceAddress? ResidenceAddress { get; set; }

    public bool HasDisability { get; set; }
    public string? DisabilityDetails { get; set; }

    public Guid? SponsorProfileId { get; set; }
    public SponsorProfile? SponsorProfile { get; set; }

    public Guid? BirthdayCertificateId { get; set; }
    public Resource? BirthdayCertificate { get; set; }

    public Guid? MarriageCertificateId { get; set; }
    public Resource? MarriageCertificate { get; set; }

    public ICollection<Qualification>? Qualifications { get; set; } = [];
    public ICollection<Experience>? Experiences { get; set; } = [];
    public ICollection<TrainingCourse>? TrainingCourses { get; set; } = [];
    public ICollection<Achievement>? Achievements { get; set; } = [];
    public ICollection<ProfileSkill>? Skills { get; set; } = [];
    public ICollection<ProfileLanguage>? Languages { get; set; } = [];
    public ICollection<ProfileAdditionalAttachment>? AdditionalAttachments { get; set; } = [];
    public ICollection<ProfileAssignment> ProfileAssignments { get; set; } = [];

    public UserProfileStatus Status { get; set; } = UserProfileStatus.InCreation;

    public bool AvailableForRecruitment { get; set; } = true;

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
}
