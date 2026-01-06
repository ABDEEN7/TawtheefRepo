using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;

internal static class JobCandidatesQueryBuilder
{
    public static IQueryable<JobCandidateRecord> BuildEligibleQuery(
        IUnitOfWork unitOfWork,
        Domain.Entities.Recruitment.Job job,
        JobRequirements req,
        JobCandidatesFilter? filter)
    {
        filter ??= new JobCandidatesFilter(null, null, null);
        var searchTerm = filter.SearchTerm?.Trim();

        var invitationsForJob = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(i => i.JobId == job.Id);

        var profiles = unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(p => p.Status == UserProfileStatus.Approved && p.AvailableForRecruitment)
            .Where(p => !invitationsForJob.Any(i => i.ApplicantId == p.UserId));

        // Gender (job + filter)
        if (job.GenderId is not null && job.GenderId.Value != GenderIds.All)
            profiles = profiles.Where(p => p.GenderId == job.GenderId.Value);

        if (filter.GenderId.HasValue && filter.GenderId.Value != GenderIds.All)
            profiles = profiles.Where(p => p.GenderId == filter.GenderId.Value);

        // Age (BirthDate window)
        profiles = profiles.Where(p => p.BirthDate.HasValue);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minBirthDate = today.AddYears(-job.MaximumAge);
        var maxBirthDate = today.AddYears(-job.MinimumAge);

        profiles = profiles.Where(p =>
            p.BirthDate!.Value >= minBirthDate &&
            p.BirthDate!.Value <= maxBirthDate);

        // Latest Major MUST match job major/submajor
        if (req.JobMajorId.HasValue || req.JobSubMajorId.HasValue)
        {
            profiles = profiles.Where(p =>
                p.Qualifications != null &&
                (
                    p.Qualifications
                        .OrderByDescending(q => q.GraduationYear)
                        .Select(q => q.MajorId)
                        .FirstOrDefault() == req.JobMajorId
                    ||
                    p.Qualifications
                        .OrderByDescending(q => q.GraduationYear)
                        .Select(q => q.MajorId)
                        .FirstOrDefault() == req.JobSubMajorId
                ));
        }

        // Required skills: MUST have ALL required skills (ProfileSkill.SkillId)
        if (req.RequiredSkillIds.Count > 0)
        {
            profiles = profiles.Where(p =>
                p.Skills != null &&
                p.Skills
                    .Where(s => req.RequiredSkillIds.Contains(s.SkillId))
                    .Select(s => s.SkillId)
                    .Distinct()
                    .Count() == req.RequiredSkillIds.Count);
        }

        // SearchTerm (optional) - عدّل حسب حقول ApplicantUser عندكم
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            profiles = profiles.Where(p =>
                p.User != null &&
                (
                    p.User.FullNameAr.Contains(searchTerm) ||
                    (p.User.Email != null && p.User.Email.Contains(searchTerm)) ||
                    (p.NationalNumber != null && p.NationalNumber.Contains(searchTerm))
                ));
        }

        return profiles.Select(p => new JobCandidateRecord
        {
            InvitationId = null,
            ApplicantId = p.UserId,
            Applicant = null,
            Profile = p, // light
            JobId = job.Id,
            Major = null,
            InvitationStatusId = null,
            Points = 0,
            CreatedDate = p.CreatedDate
        });
    }
}
