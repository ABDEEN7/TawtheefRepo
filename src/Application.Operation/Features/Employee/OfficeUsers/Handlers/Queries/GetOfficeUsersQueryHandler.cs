using Application.Operation.Features.Employee.OfficeUsers.DTOs;
using Application.Operation.Features.Employee.OfficeUsers.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.OfficeUsers.Handlers.Queries;

public sealed class GetOfficeUsersQueryHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    IMapper mapper)
    : IQueryHandler<GetOfficeUsersQuery, IResult<PaginatedResult<OfficeUserListItemDto>>>
{
    public async Task<IResult<PaginatedResult<OfficeUserListItemDto>>> Handle(
        GetOfficeUsersQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserId))
            return Result.Fail<PaginatedResult<OfficeUserListItemDto>>(ErrorsCodes.InvalidUserIdentifier);

        var officeAdmin = await userManager.Users
            .OfType<OfficeUser>()
            .AsNoTracking()
            .Include(user => user.Office)
            .ThenInclude(office => office!.Country)
            .FirstOrDefaultAsync(user => user.Id == currentUserId && !user.IsDeleted, cancellationToken);

        if (officeAdmin?.OfficeId is null || officeAdmin.Office is null)
            return Result.Fail<PaginatedResult<OfficeUserListItemDto>>(ErrorsCodes.OfficeAdminNotFound);

        var officeSummary = new OfficeSummaryDto
        {
            NameAr = officeAdmin.Office.NameAr,
            NameEn = officeAdmin.Office.NameEn,
            CountryNameAr = officeAdmin.Office.Country?.NameAr ?? string.Empty,
            CountryNameEn = officeAdmin.Office.Country?.NameEn ?? string.Empty,
            PhoneCountryCode = officeAdmin.Office.PhoneCountryCode,
            PhoneNumber = officeAdmin.Office.PhoneNumber
        };

        var name = request.Name?.Trim();
        var queryable = userManager.Users
            .OfType<OfficeUser>()
            .AsNoTracking()
            .Where(user => !user.IsDeleted && user.OfficeId == officeAdmin.OfficeId)
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                user => EF.Functions.Like(user.FullNameEn, $"%{name}%") ||
                        EF.Functions.Like(user.FullNameAr, $"%{name}%"));
        

        var result = await queryable
            .OrderByDescending(user => user.CreatedDate)
            .ToPaginatedListAsync<OfficeUser,OfficeUserListItemDto>(mapper, request, cancellationToken);

        result.AdditionalData = officeSummary;

        return Result.Ok(result);
    }
}
