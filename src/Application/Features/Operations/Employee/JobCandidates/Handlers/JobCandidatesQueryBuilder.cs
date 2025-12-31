using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers;

internal static class JobCandidatesQueryBuilder
{
    public static IQueryable<JobCandidateRecord> Build(IUnitOfWork unitOfWork, Guid jobId, JobCandidatesFilter? filter)
    {
        filter ??= new JobCandidatesFilter(null, null, null, null);

        var invitations = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Include(i => i.Job).ThenInclude(j => j!.Department)
            .Include(i => i.Job).ThenInclude(j => j!.JobCategory)
            .Include(i => i.Applicant)
            .Where(i => i.JobId == jobId);

        var profiles = unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.Gender)
            .Include(p => p.Qualifications!)
            .ThenInclude(q => q.Major);

        var query = invitations.Join(
            profiles,
            invitation => invitation.ApplicantId,
            profile => profile.UserId,
            (invitation, profile) => new JobCandidateRecord
            {
                InvitationId = invitation.Id,
                ApplicantId = invitation.ApplicantId,
                Applicant = invitation.Applicant,
                Profile = profile,
                Job = invitation.Job,
                Major = profile.Qualifications!
                    .OrderByDescending(q => q.GraduationYear)
                    .Select(q => q.Major)
                    .FirstOrDefault(),
                InvitationStatusId = invitation.InvitationStatusId,
                Points = 0,
                CreatedDate = invitation.CreatedDate
            });

        var searchTerm = filter.SearchTerm?.Trim();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(searchTerm), candidate =>
                (candidate.Applicant != null &&
                 (candidate.Applicant.FullNameAr.Contains(searchTerm!) ||
                  candidate.Applicant.FullNameEn.Contains(searchTerm!))) ||
                (candidate.Profile != null &&
                 !string.IsNullOrWhiteSpace(candidate.Profile.NationalNumber) &&
                 candidate.Profile.NationalNumber!.Contains(searchTerm!)))
            .WhereIf(filter.JobCategoryId.HasValue, candidate =>
                candidate.Job != null && candidate.Job.JobCategoryId == filter.JobCategoryId!.Value)
            .WhereIf(filter.CandidateTypeId.HasValue, candidate =>
                candidate.Profile != null && candidate.Profile.CandidateTypeId == filter.CandidateTypeId!.Value)
            .WhereIf(filter.MinimumPoints.HasValue, candidate =>
                candidate.Points >= filter.MinimumPoints!.Value);

        return query;
    }
}
