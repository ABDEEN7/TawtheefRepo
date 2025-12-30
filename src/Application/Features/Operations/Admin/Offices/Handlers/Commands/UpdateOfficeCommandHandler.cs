using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class UpdateOfficeCommandHandler(IUnitOfWork unitOfWork, UserManager<User> userManager)
    : IRequestHandler<UpdateOfficeCommand, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(
        UpdateOfficeCommand request,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.ExecuteInTransactionAsync<IResult<bool>>(
            ct => HandleInternal(request, ct),
            cancellationToken);
    }

    private async Task<IResult<bool>> HandleInternal(
        UpdateOfficeCommand request,
        CancellationToken ct)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>();

        var office = await officeRepo.DbSet
            .Include(o => o.OfficeUsers)
            .Include(o => o.SupportedCountries)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (office is null)
            return Result.Fail<bool>(ErrorsCodes.OfficeNotFound);

        office.NameAr = request.NameAr;
        office.NameEn = request.NameEn;

        if (!string.IsNullOrWhiteSpace(request.AdminEmail))
        {
            var adminResult = await UpdateOfficeAdminAsync(office,request.AdminEmail);
            if (adminResult.IsFailed)
                return Result.Fail<bool>(adminResult.Errors);
        }

        SyncSupportedCountries(office, request.SupportedCountryIds);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok(true);
    }

    private async Task<Result> UpdateOfficeAdminAsync(Office office, string adminEmail)
    {
        var email = adminEmail.Trim();
        var officeUserIds = office.OfficeUsers?
            .Select(u => u.Id)
            .ToList() ?? [];

        if (officeUserIds.Count == 0)
            return Result.Fail(ErrorsCodes.OfficeAdminNotFound);

        var officeAdmins = await userManager
            .GetUsersInRoleAsync(nameof(SystemRoleIds.OfficeAdmin));

        var officeAdmin = officeAdmins
            .FirstOrDefault(u => officeUserIds.Contains(u.Id));

        if (officeAdmin is null)
            return Result.Fail(ErrorsCodes.OfficeAdminNotFound);

        officeAdmin.Email = email;
        officeAdmin.NormalizedEmail = userManager.NormalizeEmail(email);
        officeAdmin.UserName = email;
        officeAdmin.NormalizedUserName = userManager.NormalizeName(email);

        var updateResult = await userManager.UpdateAsync(officeAdmin);

        if (!updateResult.Succeeded)
            return Result.Fail(updateResult.Errors.Select(e => e.Description));

        return Result.Ok();
    }

    private static void SyncSupportedCountries(
        Office office,
        IEnumerable<Guid> requestedCountryIds)
    {
        var requestedIds = requestedCountryIds
            .Distinct()
            .ToHashSet();

        var existing = office.SupportedCountries
            .ToDictionary(sc => sc.CountryId);

        // Soft-delete removed countries
        foreach (var supportedCountry in office.SupportedCountries
                     .Where(sc => !requestedIds.Contains(sc.CountryId)))
        {
            supportedCountry.IsDeleted = true;
        }

        // Add new ones
        foreach (var countryId in requestedIds.Where(countryId => !existing.ContainsKey(countryId)))
        {
            office.SupportedCountries.Add(new OfficeSupportedCountry
            {
                CountryId = countryId
            });
        }
    }
}
