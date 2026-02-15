using Cortex.Mediator.Queries;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetJobTitlesQueryHandler(IUnitOfWork unitOfWork)
    : IQueryHandler<GetJobTitlesQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetJobTitlesQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetEntityRepository<JobTitle>().DbSet
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.JobNameEn)
            .Select(x => new DropdownOptions
            {
                Id = x.Id,
                Name = x.JobNameEn,
                AdditionalData = new
                {
                    x.JobNameAr,
                    x.JobNameEn,
                    x.JobNumber
                }
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(data);
    }
}
