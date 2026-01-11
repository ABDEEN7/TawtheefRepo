using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Domain.Constants;

public static class ProfileReviewConstants
{
    public static class FieldPaths
    {
        public const string ResumeAttachmentId = nameof(UserProfile.ResumeAttachmentId);
        public const string NationalCardId = nameof(UserProfile.NationalCardId);
        public const string BirthdayCertificateId = nameof(UserProfile.BirthdayCertificateId);
        public const string MarriageCertificateId = nameof(UserProfile.MarriageCertificateId);
        public const string SponsorCardResourceId = "SponsorCardResourceId";
        public const string SponsorCardId = "SponsorCardId";
        public const string NationalAddressCertificateId = "NationalAddressCertificateId";
        public const string ResidenceAddressCertificateId = "ResidenceAddressCertificateId";
        public const string CertificateId = nameof(Qualification.CertificateId);
        public const string AttachmentResourceId = "AttachmentResourceId";
        public const string AttachmentId = nameof(ProfileAdditionalAttachment.AttachmentId);
        public const string AdditionalAttachments = "AdditionalAttachments";
    }

    public static class EntityNames
    {
        public const string Qualification = nameof(Qualification);
        public const string Experience = nameof(Experience);
        public const string TrainingCourse = nameof(TrainingCourse);
        public const string Achievement = nameof(Achievement);
        public const string Skill = "Skill";
        public const string Language = "Language";
        public const string Attachment = "Attachment";
        public const string ProfileAdditionalAttachment = nameof(ProfileAdditionalAttachment);
    }

    public static class AttachmentTitles
    {
        public const string BirthCertificate = "Birth Certificate";
        public const string MarriageCertificate = "Marriage Certificate";
        public const string Resume = "Resume";
        public const string NationalCard = "National Card";
        public const string SponsorCard = "Sponsor Card";
        public const string NationalAddressCertificate = "National Address Certificate";
    }
}
