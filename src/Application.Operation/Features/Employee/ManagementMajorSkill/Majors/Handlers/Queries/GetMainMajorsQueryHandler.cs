using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.DTOs;
using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Handlers.Queries;

public class GetMainMajorsQueryHandler(IUnitOfWork uow, ILocalizationService localized)
    : IQueryHandler<GetMainMajorsQuery, IResult<PaginatedResult<MajorDetailsDto>>>
{
    public async Task<IResult<PaginatedResult<MajorDetailsDto>>> Handle(
        GetMainMajorsQuery request, CancellationToken ct)
    {
        var language = localized.GetCurrentLanguage() ?? "en";
        var majorsRepo = uow.GetEntityRepository<Major>().DbSet;
        var majorSkills = uow.GetEntityRepository<MajorSkill>().DbSet;

        var search = request.Search?.Trim();
        var like = !string.IsNullOrWhiteSpace(search) ? $"%{search}%" : null;

        // Base query (main majors only? add ParentId == null if that's your definition)
        var query = majorsRepo
            .AsNoTracking()
            .Where(m => m.IsActive /* && m.ParentId == null */)
            .WhereIf(!string.IsNullOrWhiteSpace(search), m =>
                EF.Functions.Like(m.NameAr, like!) ||
                EF.Functions.Like(m.NameEn, like!) ||
                EF.Functions.Like(m.DescriptionAr ?? "", like!) ||
                EF.Functions.Like(m.DescriptionEn ?? "", like!));

        // Projection with correlated subqueries (translated to SQL)
        var projected = query.Select(m => new MajorDetailsDto
        {
            Id = m.Id,
            Name = m.GetLocalizedName(language)!,
            Description = m.GetLocalizedDescription(language)!,
            IsActive = m.IsActive,

            // direct usage for this major
            UsedInMappingsCount =
                majorSkills.Count(ms => ms.MajorId == m.Id)
                +
                // usage in all direct submajors
                majorSkills.Count(ms => majorsRepo.Any(sm => sm.Id == ms.MajorId && sm.ParentId == m.Id)),

            // number of submajors
            SubMajorsCount = majorsRepo.Count(sm => sm.ParentId == m.Id)
        });

        // paginate DTO directly (avoid mapping Major -> DTO then post-processing)
        var result = await projected.ToPaginatedListAsync(request, ct);

        return Result.Ok(result);
    }
}
