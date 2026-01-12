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

public class GetSubMajorsQueryHandler(IUnitOfWork uow, IMapper mapper) : IQueryHandler<GetSubMajorsQuery, IResult<PaginatedResult<MajorDetailsDto>>>
{
    public async Task<IResult<PaginatedResult<MajorDetailsDto>>> Handle(GetSubMajorsQuery request, CancellationToken cancellationToken)
    {
        var majorUsageCounts = await uow.GetEntityRepository<MajorSkill>().DbSet.AsNoTracking()
            .GroupBy(x => x.MajorId)
            .Select(g => new { MajorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.MajorId, x => x.Count, cancellationToken);

        var majors = await uow.GetEntityRepository<Major>().DbSet.AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.ParentId == request.ParentMajorId)
            .WhereIf(!string.IsNullOrEmpty(request.Search),
                s =>
                    EF.Functions.Like(s.NameAr, $"%{request.Search}%") ||
                    EF.Functions.Like(s.NameEn, $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionAr ?? "", $"%{request.Search}%") ||
                    EF.Functions.Like(s.DescriptionEn ?? "", $"%{request.Search}%"))
            .ToPaginatedListAsync<Major, MajorDetailsDto>(mapper, request, cancellationToken);

        foreach (var major in majors.Items)
        {
            major.UsedInMappingsCount = majorUsageCounts.TryGetValue(major.Id, out var count) ? count : 0;
        }

        return Result.Ok(majors);
    }
}
