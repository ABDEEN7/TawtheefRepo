using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Users;

[Table(nameof(UserProfile), Schema = Schemas.Applicant)]
public class UserProfile : EventEntity
{
    public Guid UserId { get; set; }
    public ApplicantUser? User { get; set; }
    
    public Guid CandidateTypeId { get; set; }
    public CandidateType? CandidateType { get; set; }
    
    public Guid TargetEntityId { get; set; }
    public TargetEntity? TargetEntity { get; set; }
    
    public Guid? ResumeAttachmentId { get; set; }
    public Resource? ResumeAttachment { get; set; }
    
    public Guid? NationalCardId { get; set; }
    public Resource? NationalCard { get; set; }
    
    /// <summary>
    /// National ID number (e.g. QID)
    /// </summary>
    public string? NationalNumber { get; set; }
    public DateOnly? BirthDate { get; set; }
    [NotMapped]
    public int? Age {
        get
        {
            if (BirthDate is null) return null;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - BirthDate!.Value.Year;
            if (today < new DateOnly(today.Year, BirthDate!.Value.Month, BirthDate!.Value.Day)) age--;
            return age;
        }
    }
    
    public Guid NationalityId { get; set; }
    public Country? Nationality { get; set; }
    
    public Guid? GenderId { get; set; }
    public Gender? Gender { get; init; }
    
    public Guid? ReligionId { get; set; }
    public Religion? Religion { get; init; }
    
    public Guid MaritalStatusId { get; set; }
    public MaritalStatus? MaritalStatus { get; set; }
    
    public int ChildrenCount { get; set; }
    
    
    public Guid ResidenceCountryId { get; set; }
    public Country? ResidenceCountry { get; set; }
    
    public Guid? InterviewLocationId { get; set; }
    public Country? InterviewLocation { get; set; }
    
    public string? Address { get; set; }
    
    public Guid ResidenceAddressId { get; set; }
    public ResidenceAddress? ResidenceAddress { get; set; }
    
    public Guid? ResidenceAddressCertificateId { get; set; }
    public Resource? ResidenceAddressCertificate { get; set; }
    
    public bool HasDisability { get; set; }
    public string? DisabilityDetails { get; set; }
    
    public Guid? SponsorProfileId { get; set; }
    public SponsorProfile? SponsorProfile { get; set; }
    
    public Guid? BirthdayCertificateId { get; set; }
    public Resource? BirthdayCertificate { get; set; }
    
    public Guid? MarriageCertificateId { get; set; }
    public Resource? MarriageCertificate { get; set; }
    
    public bool IsDraft { get; set; } = true;
    
    public ICollection<Qualification>? Qualifications { get; set; } = [];
    public ICollection<Experience>? Experiences { get; set; } = [];
    public ICollection<Achievement>? Achievements { get; set; } = [];
    public ICollection<TrainingCourse>? TrainingCourses { get; set; } = [];
    public ICollection<ProfileSkill>? Skills { get; set; } = [];
    public ICollection<ProfileLanguage>? Languages { get; set; } = [];
    public ICollection<ProfileAdditionalAttachment>? AdditionalAttachments { get; set; } = [];
}
