using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobRepository : IBaseRepository<Job>
{
    Task<IResult<Job>> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);
    Task<IResult<Job>> GetByIdWithDetailsUnTrackingAsync(Guid id);
    Task<Job?> LoadJobWithPointsAsync(Guid jobId);
    Task<List<Job>> GetJobsToAutoCloseBatchAsync(DateTimeOffset currentDate, int batchSize);
    Task<IResult<PaginatedResult<Job>>> GetFilteredJobsAsync(
        JobQueryFilter filter,
        PaginatedRequest pagination);
}
