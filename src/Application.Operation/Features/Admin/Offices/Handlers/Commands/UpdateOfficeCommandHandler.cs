using Application.Operation.Features.Admin.Offices.Commands;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Admin.Offices.Handlers.Commands;

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
            .IgnoreQueryFilters()
            .Include(o => o.OfficeUsers)
            .Include(o => o.SupportedCountries)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (office is null)
            return Result.Fail<bool>(ErrorsCodes.OfficeNotFound);

        office.NameAr = request.NameAr;
        office.NameEn = request.NameEn;
        office.PhoneCountryCode = request.PhoneCountryCode;
        office.PhoneNumber = request.PhoneNumber;

        if (!string.IsNullOrWhiteSpace(request.AdminEmail))
        {
            var adminResult = await UpdateOfficeAdminAsync(
                office,
                request.AdminEmail,
                request.AdminNameAr,
                request.AdminNameEn);
            if (adminResult.IsFailed)
                return Result.Fail<bool>(adminResult.Errors);
        }

        var requestedCountries = request.SupportedCountryIds;
        if (!requestedCountries.Any())
        {
            requestedCountries = new[] {office.CountryId};
        }

        SyncSupportedCountries(office, requestedCountries);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok(true);
    }

    private async Task<Result> UpdateOfficeAdminAsync(
        Office office,
        string adminEmail,
        string adminNameAr,
        string adminNameEn)
    {
        var email = adminEmail.Trim();
        var officeUserIds = office.OfficeUsers?
            .Select(u => u.Id)
            .ToList() ?? [];

        if (officeUserIds.Count == 0)
            return Result.Fail(ErrorsCodes.OfficeAdminNotFound);

        var officeAdmin = await userManager.Users
            .OfType<OfficeUser>()
            .FirstOrDefaultAsync(u => u.Id == office.OfficeAdminId);

        if (officeAdmin is null)
            return Result.Fail(ErrorsCodes.OfficeAdminNotFound);

        if (officeAdmin.Email != email)
        {
            var existingUser = await userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser is not null && existingUser.Id != officeAdmin.Id)
            {
                if (existingUser.UserTypeId == UserTypeIds.Applicant)
                    return Result.Fail(ErrorsCodes.UserIsApplicant);

                return Result.Fail(ErrorsCodes.EmailAlreadyInUse);
            }

            // Remove external logins (e.g. Google ProviderKey) so they
            // won't resolve to this user with the old email on future logins
            var logins = await userManager.GetLoginsAsync(officeAdmin);
            foreach (var login in logins)
                await userManager.RemoveLoginAsync(officeAdmin, login.LoginProvider, login.ProviderKey);
        }

        officeAdmin.Email = email;
        officeAdmin.NormalizedEmail = userManager.NormalizeEmail(email);
        officeAdmin.UserName = email;
        officeAdmin.NormalizedUserName = userManager.NormalizeName(email);
        officeAdmin.FullNameAr = adminNameAr;
        officeAdmin.FullNameEn = adminNameEn;

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

        // Process all requested countries
        foreach (var countryId in requestedIds)
        {
            if (existing.TryGetValue(countryId, out var existingSC))
            {
                existingSC.IsDeleted = false;
            }
            else
            {
                office.SupportedCountries.Add(new OfficeSupportedCountry
                {
                    CountryId = countryId
                });
            }
        }

        // Soft-delete those not in the request
        foreach (var sc in office.SupportedCountries.Where(sc => !requestedIds.Contains(sc.CountryId)))
        {
            sc.IsDeleted = true;
        }
    }
}

