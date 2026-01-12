using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.DTOs;
using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Handlers.Queries;

public class GetMainMajorsQueryHandler(IUnitOfWork uow, IMapper mapper) : IQueryHandler<GetMainMajorsQuery, IResult<PaginatedResult<MajorDetailsDto>>>
{
    public async Task<IResult<PaginatedResult<MajorDetailsDto>>> Handle(GetMainMajorsQuery request, CancellationToken cancellationToken)
    {
        var majorUsageCounts = await uow.GetEntityRepository<MajorSkill>().DbSet.AsNoTracking()
            .GroupBy(x => x.MajorId)
            .Select(g => new { MajorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.MajorId, x => x.Count, cancellationToken);

        var subMajorIdsByParent = await uow.GetEntityRepository<Major>().DbSet.AsNoTracking()
            .Where(x => x.ParentId != null)
            .Select(x => new { x.Id, x.ParentId })
            .ToListAsync(cancellationToken);

        var childUsageTotals = subMajorIdsByParent
            .GroupBy(x => x.ParentId!.Value)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(x => majorUsageCounts.TryGetValue(x.Id, out var count) ? count : 0));

        var majors = await uow.GetEntityRepository<Major>().DbSet.AsNoTracking()
            .Where(x => x.IsActive)
            .WhereIf(!string.IsNullOrEmpty(request.Search),
                s =>
                    EF.Functions.Like(s.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(s.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{request.Search}%"))
            .ToPaginatedListAsync<Major, MajorDetailsDto>(mapper, request, cancellationToken);

        foreach (var major in majors.Items)
        {
            var directCount = majorUsageCounts.TryGetValue(major.Id, out var count) ? count : 0;
            var childCount = childUsageTotals.TryGetValue(major.Id, out var total) ? total : 0;

            major.UsedInMappingsCount = directCount + childCount;
        }

        return Result.Ok(majors);
    }
}
