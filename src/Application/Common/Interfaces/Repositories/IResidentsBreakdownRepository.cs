using FluentResults;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Interfaces.Repositories;

public interface IResidentsBreakdownRepository : IBaseRepository<ResidentBreakdown>
{
    Task<IResult<List<ResidentBreakdown>>> GetByQuotaIdAsync(Guid quotaId);
}
