using Application.Operation.Features.Admin.Languages.DTOs;
using Application.Operation.Features.Admin.Languages.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Languages.Handlers.Queries;

public sealed class ListLanguagesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IQueryHandler<GetListLanguagesQuery, IResult<PaginatedResult<LanguageAdminDto>>>
{
    public async Task<IResult<PaginatedResult<LanguageAdminDto>>> Handle(
        GetListLanguagesQuery request,
        CancellationToken cancellationToken)
    {
        var searchTerm = request.Search?.Trim();

        var languages = await unitOfWork
            .GetEntityRepository<Language>()
            .DbSet
            .AsNoTracking()
            .WhereIf(
                !string.IsNullOrWhiteSpace(searchTerm),
                l => EF.Functions.Like(l.NameEn, $"%{searchTerm}%") ||
                     EF.Functions.Like(l.NameAr, $"%{searchTerm}%"))
            .OrderBy(l => l.DisplayOrder)
            .ThenBy(l => l.NameEn)
            .ToPaginatedListAsync<Language, LanguageAdminDto>(mapper, request, cancellationToken);

        return Result.Ok(languages);
    }
}
