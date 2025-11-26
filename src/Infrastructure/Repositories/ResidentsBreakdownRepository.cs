using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Constants;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class ResidentsBreakdownRepository(IGenericRepository<ResidentBreakdown> repository)
    : BaseRepository<ResidentBreakdown>(repository), IResidentsBreakdownRepository
{
    private readonly IGenericRepository<ResidentBreakdown> _repository = repository;

    public async Task<IResult<List<ResidentBreakdown>>> GetByQuotaIdAsync(Guid quotaId)
    {
            var residentsBreakdowns = await _repository.DbSet
                .Where(js => js.JobQuotaId == quotaId)
                .ToListAsync();
                return residentsBreakdowns.Count == 0 ? Result.Fail<List<ResidentBreakdown>>(JobValidationMessages.JobResidentsBreakdownNotFound) : Result.Ok(residentsBreakdowns);
    }
}
