using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Offices.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Queries;

public sealed class ListOfficesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
    : IRequestHandler<GetListOfficesQuery, IResult<PaginatedResult<OfficeDto>>>
{
    public async Task<IResult<PaginatedResult<OfficeDto>>> Handle(
        GetListOfficesQuery request,
        CancellationToken cancellationToken)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>().DbSet
            .Include(o => o.Country)
            .Include(o => o.SupportedCountries)!.ThenInclude(sc => sc.Country)
            .Include(o => o.OfficeUsers);

        var query = officeRepo.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(o => o.NameAr.Contains(term) || o.NameEn.Contains(term) || o.Code.Contains(term));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = request.PageSize <= 0 ? totalCount : request.PageSize;
        var skip = (pageNumber - 1) * pageSize;

        var offices = await query
            .OrderBy(o => o.DisplayOrder)
            .ThenBy(o => o.NameEn)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = mapper.Map<List<OfficeDto>>(offices);

        var officeUsersInRole = await userManager.GetUsersInRoleAsync(SystemRoles.OfficeAdmin);
        var adminsLookup = officeUsersInRole
            .OfType<OfficeUser>()
            .Where(u => !u.IsDeleted)
            .GroupBy(u => u.OfficeId)
            .ToDictionary(g => g.Key, g => g.First().Email ?? string.Empty);

        items = items
            .Select(o => o with
            {
                AdminEmail = adminsLookup.TryGetValue(o.Id, out var email)
                    ? email ?? string.Empty
                    : string.Empty
            })
            .ToList();

        return Result.Ok(new PaginatedResult<OfficeDto>(items, totalCount, pageNumber, pageSize));
    }
}
