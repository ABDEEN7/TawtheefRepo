using System.Linq.Expressions;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Employee.Job.Queries;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IJobRepository : IBaseRepository<Job>
{
    Task<IResult<Job>> GetByIdAsync(Guid id, params Expression<Func<Job, object>>[]? includes);
    
    Task<IResult<Job>> GetByIdWithDetailsAsync(Guid id);
    
    Task<IResult<PaginatedResult<Job>>> GetPaginatedJobsAsync(PaginatedRequest request);
    Task<IResult<PaginatedResult<Job>>> GetFilteredJobsAsync(JobQueryFilter filter, PaginatedRequest pagination);
}
