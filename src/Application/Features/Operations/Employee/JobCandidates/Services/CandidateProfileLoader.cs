using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Employee.JobCandidates.Services;

internal static class CandidateProfileLoader
{
    public static async Task<List<UserProfile>> LoadForScoringAsync(
        IUnitOfWork unitOfWork,
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct)
    {
        if (userIds.Count == 0) return [];

        return await unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(p => userIds.Contains(p.UserId))
            .Include(p => p.User)
            .Include(p => p.CandidateType)
            .Include(p => p.Gender)
            .Include(p => p.Nationality)
            .Include(p => p.Qualifications!).ThenInclude(q => q.Major)
            .Include(p => p.Qualifications!).ThenInclude(q => q.Degree)
            .Include(p => p.Qualifications!).ThenInclude(q => q.University)
            .Include(p => p.Experiences)
            .Include(p => p.TrainingCourses)
            .Include(p => p.Achievements)
            .Include(p => p.Skills)
            .Include(p => p.Languages)
            .ToListAsync(ct);
    }
}
