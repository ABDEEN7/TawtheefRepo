using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;

public class JobCandidatesQueryBuilderService(IUnitOfWork unitOfWork) : IJobCandidatesQueryBuilderService
{
    public IQueryable<JobCandidateRecord> BuildEligibleQuery(Guid jobId, Guid jobTargetId,
        Guid? jobGenderId, int jobMaximumAge,
        int jobMinimumAge,JobRequirements req, JobCandidatesFilter? filter)
    {

        filter ??= new JobCandidatesFilter(null, null, null);
        var searchTerm = filter.SearchTerm?.Trim();

        var activeInvitationStatuses = new[]
        {
            InvitationStatusIds.NewInvitation,
            InvitationStatusIds.Read,
            InvitationStatusIds.Submitted
        };

        var invitationsForJob = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(i => i.JobId == jobId && activeInvitationStatuses.Contains(i.InvitationStatusId));

        var profiles = unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(p => p.Status == UserProfileStatus.Approved && p.AvailableForRecruitment)
            .Where(p => p.TargetEntityId == jobTargetId)
            .Where(p => !invitationsForJob.Any(i => i.ApplicantId == p.UserId));

        // Gender (job + filter)
        if (jobGenderId is not null && jobGenderId != GenderIds.All)
            profiles = profiles.Where(p => p.GenderId == jobGenderId);

        if (filter.GenderId.HasValue && filter.GenderId.Value != GenderIds.All)
            profiles = profiles.Where(p => p.GenderId == filter.GenderId.Value);

        // Age (BirthDate window)
        profiles = profiles.Where(p => p.BirthDate.HasValue);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minBirthDate = today.AddYears(-jobMaximumAge);
        var maxBirthDate = today.AddYears(-jobMinimumAge);

        profiles = profiles.Where(p =>
            p.BirthDate!.Value >= minBirthDate &&
            p.BirthDate!.Value <= maxBirthDate);

        profiles = profiles.Where(p =>
            p.Qualifications!.Any(pq => req.QualificationLevelIds.Contains(pq.DegreeId)));
        
        // Latest Major MUST match job major/submajor OR at least match with one from JobSpecializations
        if (filter.SelectedSpecializations is { Count: > 0 })
        {
            var subMajorIds = filter.SelectedSpecializations.Select(s => (Guid?)s.SubMajorId).ToList();
            profiles = profiles.Where(p =>
                p.Qualifications!.Any(q =>
                    q.SubMajorId == req.JobSubMajorId || subMajorIds.Contains(q.SubMajorId)
                )
            );
        }

        //Required skills: MUST have ALL required skills (ProfileSkill.SkillId)
        if (req.RequiredSkillIds.Count > 0)
        {
            profiles = profiles.Where(p =>
                p.Skills!
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
            Profile = null,
            JobId = jobId,
            Major = null,
            InvitationStatusId = null,
            Points = 0,
            CreatedDate = p.CreatedDate
        });
    }
}
