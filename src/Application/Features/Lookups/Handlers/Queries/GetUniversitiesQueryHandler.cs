using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class GetUniversitiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetUniversitiesQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(GetUniversitiesQuery request, CancellationToken cancellationToken)
    {
        var entities = await unitOfWork.GetEntityRepository<University>().DbSet
            .AsNoTracking()
            .Where(u => u.City!.CountryId == request.CountryId)
            .WhereIf(!string.IsNullOrEmpty(request.Search),
                university =>
                    EF.Functions.Like(university.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(university.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(university.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(university.DescriptionEn ?? "", $"%{request.Search}%"))
            .OrderBy(university => university.DisplayOrder)
            .ToListAsync(cancellationToken);

        return Result.Ok(mapper.Map<List<DropdownOptions>>(entities));
    }
}

