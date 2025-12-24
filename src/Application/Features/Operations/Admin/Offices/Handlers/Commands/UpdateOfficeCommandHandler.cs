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

        var office = await officeRepo.DbSet
            .Include(a => a.OfficeAdmin)
            .Include(o => o.SupportedCountries)
            .Include(o => o.Country)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (office is null)
            return Result.Fail<OfficeDto>(ErrorsCodes.OfficeNotFound);
        
        var requestedIds = request.SupportedCountryIds
            .Distinct()
            .ToHashSet();
        
        office.NameAr = request.NameAr;
        office.NameEn = request.NameEn;

        #region Update Office Admin
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
                var officeAdmins = await userManager
                    .GetUsersInRoleAsync(nameof(SystemRoleIds.OfficeAdmin));

                officeAdmin = officeAdmins
                    .FirstOrDefault(u => officeUserIds.Contains(u.Id));
            }
            
            if (officeAdmin is null)
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeAdminNotFound);

            officeAdmin.Email = adminEmail;
            officeAdmin.UserName = adminEmail;
            officeAdmin.NormalizedEmail = userManager.NormalizeEmail(adminEmail);
            officeAdmin.NormalizedUserName = userManager.NormalizeName(adminEmail);

            var updateResult = await userManager.UpdateAsync(officeAdmin);

            if (!updateResult.Succeeded)
                return Result.Fail<OfficeDto>(updateResult.Errors.Select(e => e.Description).ToArray());
            
        }
        #endregion

        #region Sync Supported Countries
        var existingByCountry = office.SupportedCountries
            .ToDictionary(sc => sc.CountryId);

        var toRemove = office.SupportedCountries
            .Where(sc => !requestedIds.Contains(sc.CountryId))
            .ToList();

        unitOfWork.RemoveRange(toRemove);

        foreach (var countryId in request.SupportedCountryIds)
        {
            if (existingByCountry.ContainsKey(countryId))
                continue;

            office.SupportedCountries.Add(new OfficeSupportedCountry { CountryId = countryId });
        }
        #endregion
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(mapper.Map<OfficeDto>(office));
    }
}
