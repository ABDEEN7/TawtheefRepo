using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobRepository : IBaseRepository<Job>
{
    Task<IResult<Job>> GetByIdWithDetailsAsync(Guid id);
    Task<IResult<Job>> GetByIdWithDetailsUnTrackingAsync(Guid id);
    Task<Job?> LoadJobWithPointsAsync(Guid jobId);
    Task<IList<Job>> GetJobsToAutoCloseAsync(DateTimeOffset currentDate);
    Task<IResult<PaginatedResult<Job>>> GetFilteredJobsAsync(
        JobQueryFilter filter,
        PaginatedRequest pagination);
}
