using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Lookups.Queries;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Lookups.Handlers.Queries;

public sealed class SearchSkillsHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<SearchSkillsQuery, IResult<List<DropdownOptions>>>
{

    public async Task<IResult<List<DropdownOptions>>> Handle(
        SearchSkillsQuery request,
        CancellationToken cancellationToken)
    {
        var term = request.Search?.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(term) || term.Length < 3) 
            return Result.Ok(new List<DropdownOptions>());

        var matches = await uow.GetEntityRepository<SkillType>().DbSet
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Where(s =>
                EF.Functions.Like(s.NameAr, $"%{term}%") ||
                EF.Functions.Like(s.NameEn, $"%{term}%") ||
                EF.Functions.Like(s.DescriptionAr ?? "", $"%{term}%") ||
                EF.Functions.Like(s.DescriptionEn ?? "", $"%{term}%"))
            .OrderBy(s => s.DisplayOrder)
            .Take(10)
            .ToListAsync(cancellationToken);
        return Result.Ok(mapper.Map<List<DropdownOptions>>(matches));
    }
}
