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
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class CreateOfficeCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper)
    : IRequestHandler<CreateOfficeCommand, IResult<OfficeDto>>
{
    public async Task<IResult<OfficeDto>> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var officeRepo = unitOfWork.GetEntityRepository<Office>();
            var countryRepo = unitOfWork.GetEntityRepository<Country>();

            var adminEmail = request.AdminEmail.Trim();

            if (string.IsNullOrWhiteSpace(adminEmail)
                || !new EmailAddressAttribute().IsValid(adminEmail))
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeAdminEmailInvalid);
            }

            var existingAdmin = await userManager.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == adminEmail, cancellationToken);
            if (existingAdmin is not null)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeAdminEmailExists);
            }

            var country = await countryRepo.DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CountryId, cancellationToken);
            if (country is null)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeCountryNotFound);
            }

            var supportedIds = request.SupportedCountryIds.Distinct().ToArray();

            var supportedCountries = await countryRepo.DbSet
                .AsNoTracking()
                .Where(c => supportedIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            if (supportedIds.Length != supportedCountries.Count)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeSupportedCountryInvalid);
            }

            var backendName = GenerateBackendName(request.NameEn);
            var officeCode = await GenerateUniqueOfficeCodeAsync(officeRepo.DbSet, country.CodeAlpha, cancellationToken);

            var office = new Office
            {
                Id = Guid.NewGuid(),
                BackendName = backendName,
                NameAr = request.NameAr,
                NameEn = request.NameEn,
                CountryId = country.Id,
                Country = country,
                Code = officeCode,
                SupportedCountries = supportedCountries
                    .Select((c, index) => new OfficeSupportedCountry
                    {
                        Id = Guid.NewGuid(),
                        BackendName = $"{backendName}_SUP_{c.CodeAlpha}",
                        NameAr = c.NameAr,
                        NameEn = c.NameEn,
                        DescriptionAr = c.DescriptionAr,
                        DescriptionEn = c.DescriptionEn,
                        CountryId = c.Id,
                        DisplayOrder = index + 1
                    })
                    .ToList()
            };

            var addResult = await officeRepo.AddAsync(office);
            if (addResult.IsFailed)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(addResult.Errors);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var officeAdmin = new OfficeUser
            {
                Id = Guid.NewGuid(),
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

            var createResult = await userManager.CreateAsync(officeAdmin);
            if (!createResult.Succeeded)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeAdminCreationFailed);
            }

            var officeAdminRoleId = await roleManager.Roles
                .Where(r => r.Name == SystemRoles.OfficeAdmin)
                .Select(r => r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (officeAdminRoleId == Guid.Empty)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeRoleAssignmentFailed);
            }

            var role = await roleManager.FindByIdAsync(officeAdminRoleId.ToString());
            if (role is null)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeRoleAssignmentFailed);
            }

            var addRoleResult = await userManager.AddToRoleAsync(officeAdmin, SystemRoles.OfficeAdmin);
            if (!addRoleResult.Succeeded)
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Fail<OfficeDto>(ErrorsCodes.OfficeRoleAssignmentFailed);
            }

            await unitOfWork.CommitTransactionAsync(cancellationToken);

            var dto = mapper.Map<OfficeDto>(office);
            dto = dto with { AdminEmail = officeAdmin.Email ?? string.Empty };

            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<OfficeDto>(ex.Message);
        }
    }

    private static string GenerateBackendName(string nameEn)
    {
        var sanitized = new string(nameEn.Where(char.IsLetterOrDigit).ToArray());
        sanitized = string.IsNullOrWhiteSpace(sanitized) ? "office" : sanitized;
        return $"{sanitized}_" + Guid.NewGuid().ToString("N");
    }

    private static async Task<string> GenerateUniqueOfficeCodeAsync(
        IQueryable<Office> offices,
        string countryCode,
        CancellationToken cancellationToken)
    {
        string code;
        do
        {
            code = $"{countryCode.ToUpperInvariant()}-OFF-{Random.Shared.Next(1000, 9999)}";
        } while (await offices.AnyAsync(o => o.Code == code, cancellationToken));

        return code;
    }

}
