using Application.Operation.Features.Admin.Users.Queries;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Admin.Users.Handlers.Queries;

public sealed class GetUsersLookupQueryHandler(
    UserManager<User> userManager)
    : IRequestHandler<GetUsersLookupQuery, IResult<List<DropdownOptions>>>
{
    public async Task<IResult<List<DropdownOptions>>> Handle(
        GetUsersLookupQuery request,
        CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .AsNoTracking()
            .Where(u => u.UserTypeId == UserTypeIds.Employee)
            .OrderBy(u => u.FullNameEn)
            .Select(u => new DropdownOptions
            {
                Id = u.Id,
                Name = !string.IsNullOrWhiteSpace(u.FullNameAr) ? u.FullNameAr : u.FullNameEn,
                BackendName = u.FullNameEn,
                AdditionalData = new { u.FullNameAr, u.FullNameEn }
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(users);
    }
}
