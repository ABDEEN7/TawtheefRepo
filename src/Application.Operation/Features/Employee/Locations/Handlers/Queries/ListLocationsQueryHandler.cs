using Application.Operation.Features.Employee.Locations.DTOs;
using Application.Operation.Features.Employee.Locations.Queries;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Operation.Features.Employee.Locations.Handlers.Queries;

public sealed class ListLocationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<ListLocationsQuery, IResult<PaginatedResult<LocationDto>>>
{
    public async Task<IResult<PaginatedResult<LocationDto>>> Handle(
        ListLocationsQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.Search?.Trim();

        var locations = await unitOfWork.GetEntityRepository<Location>().DbSet
            .AsNoTracking()
            .WhereIf(
                !string.IsNullOrWhiteSpace(searchTerm),
                location => EF.Functions.Like(location.NameAr, $"%{searchTerm}%") ||
                            (location.NameEn != null &&
                             EF.Functions.Like(location.NameEn, $"%{searchTerm}%")))
            .OrderBy(location => location.NameAr)
            .ToPaginatedListAsync<Location, LocationDto>(mapper, request, cancellationToken);

        return Result.Ok(locations);
    }
}
