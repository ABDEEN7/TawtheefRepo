using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Application.Features.Operations.Admin.Offices.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Queries;

public sealed class GetOfficeDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    UserManager<User> userManager)
    : IRequestHandler<GetOfficeDetailsQuery, IResult<OfficeDetailsDto>>
{
    public async Task<IResult<OfficeDetailsDto>> Handle(GetOfficeDetailsQuery request, CancellationToken cancellationToken)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>();

        var office = await officeRepo.DbSet
            .Include(o => o.Country)
            .Include(o => o.SupportedCountries)!
                .ThenInclude(sc => sc.Country)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (office is null)
            return Result.Fail<OfficeDetailsDto>(ErrorsCodes.OfficeNotFound);

        var officeUsers = office.OfficeUsers?.Where(u => !u.IsDeleted).OfType<OfficeUser>().ToList() ?? [];
        var officeUserIds = officeUsers.Select(u => u.Id).ToHashSet();

        var adminsInRole = await userManager.GetUsersInRoleAsync(SystemRoles.OfficeAdmin);
        var officeAdmin = adminsInRole
            .OfType<OfficeUser>()
            .Where(u => !u.IsDeleted && officeUserIds.Contains(u.Id))
            .FirstOrDefault();

        var adminId = officeAdmin?.Id;
        var adminEmail = officeAdmin?.Email ?? string.Empty;

        var details = mapper.Map<OfficeDetailsDto>(office);
        details = details with
        {
            AdminEmail = adminEmail,
            Users = officeUsers
                .Select(u => new OfficeUserDto
                {
                    Id = u.Id,
                    FullNameAr = u.FullNameAr,
                    FullNameEn = u.FullNameEn,
                    Email = u.Email ?? string.Empty,
                    IsBlocked = u.IsBlocked,
                    IsAdmin = adminId.HasValue && adminId == u.Id
                })
                .OrderByDescending(u => u.IsAdmin)
                .ThenBy(u => u.FullNameEn)
                .ToList()
        };

        return Result.Ok(details);
    }
}
