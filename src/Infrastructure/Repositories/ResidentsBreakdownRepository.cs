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
}
