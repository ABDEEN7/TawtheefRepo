using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Admin.Universities.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Universities.Queries;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Universities.Handlers.Queries;

public sealed class ListUniversitiesQueryHandler(IUnitOfWork unitOfWork, ILocalizationService localizationService)
    : IRequestHandler<GetListUniversitiesQuery, IResult<PaginatedResult<UniversityAdminDto>>>
{
    public async Task<IResult<PaginatedResult<UniversityAdminDto>>> Handle(
        GetListUniversitiesQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.Search?.Trim();

        var universities = await unitOfWork
            .GetEntityRepository<University>()
            .DbSet
            .AsNoTracking()
            .WhereIf(
                !string.IsNullOrWhiteSpace(searchTerm),
                u => EF.Functions.Like(u.NameEn, $"%{searchTerm}%") ||
                     EF.Functions.Like(u.NameAr, $"%{searchTerm}%"))
            .WhereIf(
                request.CountryId.HasValue,
                u => u.City != null && u.City.CountryId == request.CountryId)
            .Select(u => new UniversityAdminDto
            {
                Id = u.Id,
                NameEn = u.NameEn,
                NameAr = u.NameAr,
                CountryId = u.City!.CountryId,
                CountryName = localizationService.GetLocalizedName(u.City!.Country!),
                CityId = u.CityId,
                CityName = localizationService.GetLocalizedName(u.City!),
                Code = u.Code,
                Email = u.Email,
                Phone = u.Phone,
                WebSite = u.WebSite,
                IsActive = u.IsActive,
                DescriptionEn = u.DescriptionEn,
                DescriptionAr = u.DescriptionAr,
                OriginalName = u.OriginalName
            })
            .ToPaginatedListAsync<UniversityAdminDto>(request, cancellationToken);

        return Result.Ok(universities);
    }
}
