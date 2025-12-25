using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile;

public static class UserProfileLoader
{
    public static async Task<IResult<UserProfile?>> GetSummaryAsync(IUnitOfWork uow, Guid userId, CancellationToken ct) {
        var repo = uow.GetEntityRepository<UserProfile>();
        var profile = await repo.DbSet.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        return Result.Ok(profile);
    }

    public static async Task<UserProfile?> GetFullProfile(IUnitOfWork uow, Guid userId, bool tracking = false, CancellationToken ct = default) {
        var repo = uow.GetEntityRepository<UserProfile>();
        var query = repo.DbSet.AsSplitQuery()
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
            .Include(p => p.ResidenceAddress).ThenInclude(r => r!.Certificate)
            
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

            .Include(p => p.AdditionalAttachments)!.ThenInclude(a => a.Attachment).AsQueryable();
        
        if (!tracking)
            query = query.AsTracking();
        
        var profile = await query.FirstOrDefaultAsync(p => p.UserId == userId, ct);

        return profile;
    }
}
