using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.ProfileManagement;

internal static class UserProfileQueryFactory
{
    public static IQueryable<UserProfile> CreateSectionQuery(IUnitOfWork uow, ProfileSection? section)
    {
        var query = CreateBaseQuery(uow, tracking: false);
        return IncludeSectionData(query, section);
    }

    public static IQueryable<UserProfile> CreateFullQuery(IUnitOfWork uow, bool tracking)
    {
        var query = CreateBaseQuery(uow, tracking);
        return IncludeAllSections(query);
    }

    private static IQueryable<UserProfile> CreateBaseQuery(IUnitOfWork uow, bool tracking)
    {
        var query = uow.GetEntityRepository<UserProfile>().DbSet.AsSplitQuery();
        query = tracking ? query.AsTracking() : query.AsNoTracking();

        return query
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.TargetEntity)
            .Include(p => p.ResumeAttachment)
            .Include(p => p.NationalCard)
            .Include(p => p.Office)
            .Include(p => p.BirthdayCertificate)
            .Include(p => p.MarriageCertificate)
            .Include(p => p.Nationality)
            .Include(p => p.Gender)
            .Include(p => p.Religion)
            .Include(p => p.MaritalStatus)
            .Include(p => p.SponsorProfile!.SponsorCard)
            .Include(p => p.SponsorProfile!.SponsorType)
            .Include(p => p.ResidenceCountry)
            .Include(p => p.InterviewLocation)
            .Include(p => p.ResidenceAddress).ThenInclude(r => r!.Certificate);
    }

    private static IQueryable<UserProfile> IncludeSectionData(IQueryable<UserProfile> query, ProfileSection? section)
    {
        return section switch
        {
            ProfileSection.Qualifications => query
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Degree)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Country)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.SubMajor)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Rating)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.StudyType)
                .Include(p => p.Qualifications)!.ThenInclude(q => q.Certificate),
            ProfileSection.Experience or ProfileSection.TrainingCourses => query
                .Include(p => p.Experiences)!.ThenInclude(e => e.Country)
                .Include(p => p.Experiences)!.ThenInclude(e => e.Qualification)
                .Include(p => p.Experiences)!.ThenInclude(e => e.Certificate)
                .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Country)
                .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Certificate),
            ProfileSection.CertificatesAndAwards => query
                .Include(p => p.Achievements)!.ThenInclude(a => a.AchievementType)
                .Include(p => p.Achievements)!.ThenInclude(a => a.Country)
                .Include(p => p.Achievements)!.ThenInclude(a => a.Attachment),
            ProfileSection.Skills => query
                .Include(p => p.Skills)!.ThenInclude(s => s.Skill)
                .Include(p => p.Skills)!.ThenInclude(s => s.Level),
            ProfileSection.Languages => query
                .Include(p => p.Languages)!.ThenInclude(l => l.Language!)
                .Include(p => p.Languages)!.ThenInclude(l => l.SpeakingLevel)
                .Include(p => p.Languages)!.ThenInclude(l => l.WritingLevel)
                .Include(p => p.Languages)!.ThenInclude(l => l.ReadingLevel),
            ProfileSection.Attachments => query
                .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment),
            _ => query
        };
    }

    private static IQueryable<UserProfile> IncludeAllSections(IQueryable<UserProfile> query)
    {
        return query
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Degree)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Country)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Major)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.SubMajor)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.University)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Rating)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.StudyType)
            .Include(p => p.Qualifications)!.ThenInclude(q => q.Certificate)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Country)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Qualification)
            .Include(p => p.Experiences)!.ThenInclude(e => e.Certificate)
            .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Country)
            .Include(p => p.TrainingCourses)!.ThenInclude(t => t.Certificate)
            .Include(p => p.Achievements)!.ThenInclude(a => a.AchievementType)
            .Include(p => p.Achievements)!.ThenInclude(a => a.Country)
            .Include(p => p.Achievements)!.ThenInclude(a => a.Attachment)
            .Include(p => p.Skills)!.ThenInclude(s => s.Skill)
            .Include(p => p.Skills)!.ThenInclude(s => s.Level)
            .Include(p => p.Languages)!.ThenInclude(l => l.Language!)
            .Include(p => p.Languages)!.ThenInclude(l => l.SpeakingLevel)
            .Include(p => p.Languages)!.ThenInclude(l => l.WritingLevel)
            .Include(p => p.Languages)!.ThenInclude(l => l.ReadingLevel)
            .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment);
    }
}
