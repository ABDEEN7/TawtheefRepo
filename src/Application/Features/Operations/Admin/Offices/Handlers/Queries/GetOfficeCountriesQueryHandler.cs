using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Offices.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Queries;

public sealed class GetOfficeCountriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetOfficeCountriesQuery, IResult<List<CountryLookupDto>>>
{
    public async Task<IResult<List<CountryLookupDto>>> Handle(
        GetOfficeCountriesQuery request,
        CancellationToken cancellationToken)
    {
        var countryEntities = await unitOfWork.GetEntityRepository<Country>().DbSet
            .AsNoTracking()
            .OrderBy(c => c.NameEn)
            .ToListAsync(cancellationToken);

        var countries = mapper.Map<List<CountryLookupDto>>(countryEntities);

        return Result.Ok(countries);
    }
}
