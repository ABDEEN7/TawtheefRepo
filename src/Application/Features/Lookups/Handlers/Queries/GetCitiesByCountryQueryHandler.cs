using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetCitiesByCountryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetCitiesByCountryQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetCitiesByCountryQuery request, CancellationToken cancellationToken)
    {
        var entities = await unitOfWork.GetEntityRepository<City>().DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Where(c => c.CountryId == request.CountryId)
            .WhereIf(!string.IsNullOrEmpty(request.Search),
                c => EF.Functions.Like(c.NameAr, $"%{request.Search}%") ||
                     EF.Functions.Like(c.NameEn, $"%{request.Search}%") ||
                     EF.Functions.Like(c.DescriptionAr ?? string.Empty, $"%{request.Search}%") ||
                     EF.Functions.Like(c.DescriptionEn ?? string.Empty, $"%{request.Search}%"))
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<DropdownOptions>>(entities).OrderBy(e => e.Name).ToList();
        return Result.Ok(data);
    }
}
