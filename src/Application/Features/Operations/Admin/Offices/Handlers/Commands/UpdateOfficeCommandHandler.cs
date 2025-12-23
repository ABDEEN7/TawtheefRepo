using System.ComponentModel.DataAnnotations;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class UpdateOfficeCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    IMapper mapper
) : IRequestHandler<UpdateOfficeCommand, IResult<OfficeDto>>
{
    public async Task<IResult<OfficeDto>> Handle(
        UpdateOfficeCommand request,
        CancellationToken cancellationToken)
    {
        var officeRepo = unitOfWork.GetEntityRepository<Office>();
        var countryRepo = unitOfWork.GetEntityRepository<Country>();

        var office = await officeRepo.DbSet
            .Include(o => o.SupportedCountries)
            .Include(o => o.Country)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (office is null)
            return Result.Fail<OfficeDto>(ErrorsCodes.OfficeNotFound);

        // -----------------------------
        // Supported Countries Validation
        // -----------------------------
        var requestedIds = request.SupportedCountryIds
            .Distinct()
            .ToHashSet();

        var supportedCountries = await countryRepo.DbSet
            .AsNoTracking()
            .Where(c => requestedIds.Contains(c.Id))
            .ToListAsync(cancellationToken);

        if (requestedIds.Count != supportedCountries.Count)
            return Result.Fail<OfficeDto>(ErrorsCodes.OfficeSupportedCountryInvalid);

        // -----------------------------
        // Update Office Basic Info
        // -----------------------------
        office.NameAr = request.NameAr;
        office.NameEn = request.NameEn;

        // -----------------------------
        // Update / Create Office Admin
        // -----------------------------
        if (!string.IsNullOrWhiteSpace(request.AdminEmail))
        {
            var adminEmail = request.AdminEmail.Trim();

            if (!new EmailAddressAttribute().IsValid(adminEmail))
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeAdminEmailInvalid);

            var emailExists = await userManager.Users
                .AnyAsync(u => u.Email == adminEmail, cancellationToken);

            var officeUserIds = office.OfficeUsers?
                .Select(ou => ou.Id)
                .ToList() ?? [];

            User? officeAdmin = null;

            if (officeUserIds.Any())
            {
                var officeUsers = await userManager.Users
                    .Where(u => officeUserIds.Contains(u.Id))
                    .ToListAsync(cancellationToken);

                foreach (var user in officeUsers)
                {
                    if (await userManager.IsInRoleAsync(user, SystemRoles.OfficeAdmin))
                    {
                        officeAdmin = user;
                        break;
                    }
                }
            }

            // -----------------------------
            // Create Office Admin if missing
            // -----------------------------
            if (officeAdmin is null)
            {
                if (emailExists)
                    return Result.Fail<OfficeDto>(ErrorsCodes.OfficeAdminEmailExists);

                var newOfficeAdmin = new OfficeUser
                {
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpperInvariant(),
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpperInvariant(),
                    FullNameAr = "مدير المكتب",
                    FullNameEn = "Office Admin",
                    UserTypeId = UserTypeIds.OfficeUser,
                    OfficeId = office.Id,
                    Office = office,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(newOfficeAdmin);

                if (!createResult.Succeeded)
                {
                    return Result.Fail<OfficeDto>(
                        createResult.Errors.Select(e => e.Description).ToArray()
                    );
                }

                var roleResult = await userManager.AddToRoleAsync(
                    newOfficeAdmin,
                    SystemRoles.OfficeAdmin);

                if (!roleResult.Succeeded)
                {
                    return Result.Fail<OfficeDto>(
                        roleResult.Errors.Select(e => e.Description).ToArray()
                    );
                }

                office.OfficeUsers ??= [];
                office.OfficeUsers.Add(newOfficeAdmin);
            }
            else
            {
                // -----------------------------
                // Update Existing Admin Email
                // -----------------------------
                if (emailExists && officeAdmin.Email != adminEmail)
                    return Result.Fail<OfficeDto>(ErrorsCodes.OfficeAdminEmailExists);

                officeAdmin.Email = adminEmail;
                officeAdmin.UserName = adminEmail;
                officeAdmin.NormalizedEmail = userManager.NormalizeEmail(adminEmail);
                officeAdmin.NormalizedUserName = userManager.NormalizeName(adminEmail);

                var updateResult = await userManager.UpdateAsync(officeAdmin);

                if (!updateResult.Succeeded)
                {
                    return Result.Fail<OfficeDto>(
                        updateResult.Errors.Select(e => e.Description).ToArray()
                    );
                }
            }
        }

        // -----------------------------
        // Sync Supported Countries
        // -----------------------------
        var existingByCountry = office.SupportedCountries
            .ToDictionary(sc => sc.CountryId);

        var toRemove = office.SupportedCountries
            .Where(sc => !requestedIds.Contains(sc.CountryId))
            .ToList();

        unitOfWork.RemoveRange(toRemove);

        foreach (var country in supportedCountries)
        {
            if (existingByCountry.ContainsKey(country.Id))
                continue;

            office.SupportedCountries.Add(new OfficeSupportedCountry { CountryId = country.Id });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(mapper.Map<OfficeDto>(office));
    }
}
