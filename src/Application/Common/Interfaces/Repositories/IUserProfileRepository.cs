using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IUserProfileRepository : IBaseRepository<UserProfile>
{
    Task<List<UserProfile>> LoadForScoringAsync(IReadOnlyCollection<Guid> userIds);
}
