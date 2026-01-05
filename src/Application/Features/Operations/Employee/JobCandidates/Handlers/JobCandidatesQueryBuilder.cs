using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.JobCandidates.Models;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Handlers;

internal static class JobCandidatesQueryBuilder
{
    public static IQueryable<JobCandidateRecord> Build(
        IUnitOfWork unitOfWork,
        Guid jobId,
        JobCandidatesFilter? filter)
    {
        filter ??= new JobCandidatesFilter(null, null, null, null);

        var invitationsForJob = unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(i => i.JobId == jobId);

        var profiles = unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.Gender)
            .Include(p => p.Qualifications!).ThenInclude(q => q.Major)
            .Include(p => p.Qualifications!).ThenInclude(q => q.Degree)
            .Include(p => p.Qualifications!).ThenInclude(q => q.University)
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .Include(p => p.Achievements)
            .Include(p => p.Skills)
            .Include(p => p.Languages);

        // LEFT JOIN: profiles -> invitations (for this job)
       var query =
           from profile in profiles
           join inv in invitationsForJob on profile.UserId equals inv.ApplicantId into invs
           from inv in invs.DefaultIfEmpty()
           where inv == null // ✅ exclude already-invited profiles for this job
           select new JobCandidateRecord
           {
               InvitationId = null, // always null now (since inv is null)
               ApplicantId = profile.UserId,
               Applicant = profile.User,
               Profile = profile,
               JobId = jobId,
       
               Major = profile.Qualifications != null
                   ? profile.Qualifications
                       .OrderByDescending(q => q.GraduationYear)
                       .Select(q => q.Major)
                       .FirstOrDefault()
                   : null,
       
               InvitationStatusId = null,
               Points = 0,
               CreatedDate = null
           };

        var searchTerm = filter.SearchTerm?.Trim();

        query = query
            .WhereIf(!string.IsNullOrWhiteSpace(searchTerm), candidate =>
                (candidate.Applicant != null &&
                 (candidate.Applicant.FullNameAr.Contains(searchTerm!) ||
                  candidate.Applicant.FullNameEn.Contains(searchTerm!))) ||
                (candidate.Profile != null &&
                 !string.IsNullOrWhiteSpace(candidate.Profile.NationalNumber) &&
                 candidate.Profile.NationalNumber!.Contains(searchTerm!)))
            .WhereIf(filter.CandidateTypeId.HasValue, candidate =>
                candidate.Profile != null && candidate.Profile.CandidateTypeId == filter.CandidateTypeId!.Value);

        return query;
    }
}
