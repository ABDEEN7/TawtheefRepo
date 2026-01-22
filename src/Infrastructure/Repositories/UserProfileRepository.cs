using Application.Operation.Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class UserProfileRepository(IGenericRepository<UserProfile> repository)
    : BaseRepository<UserProfile>(repository), IUserProfileRepository
{
    public async Task<List<UserProfile>> LoadForScoringAsync(IReadOnlyCollection<Guid> userIds,CancellationToken cancellationToken)
    {
        if (userIds.Count == 0) return [];

        return await Repository.DbSet   
            .AsNoTracking()
            .AsSplitQuery()
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
            .ToListAsync(cancellationToken);
    }
    
    public async Task<UserProfile?> LoadForScoringSingleAsync(Guid userId,CancellationToken cancellationToken)
    {
        return await Repository.DbSet   
            .AsNoTracking()
            .AsSplitQuery()
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
            .SingleOrDefaultAsync(p => p.UserId == userId,cancellationToken);
    }
}
