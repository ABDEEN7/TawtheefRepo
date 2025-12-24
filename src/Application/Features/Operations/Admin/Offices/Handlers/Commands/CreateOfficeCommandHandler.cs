using System.ComponentModel.DataAnnotations;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class CreateOfficeCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager)
    : IRequestHandler<CreateOfficeCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateOfficeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransactionAsync<IResult<Guid>>(async ct =>
            {
                var officeRepo = unitOfWork.GetEntityRepository<Office>();

                // -----------------------------
                // Validate Admin Email
                // -----------------------------
                var adminEmail = request.AdminEmail.Trim();

                if (string.IsNullOrWhiteSpace(adminEmail) || 
                    !new EmailAddressAttribute().IsValid(adminEmail))
                    return Result.Fail<Guid>(ErrorsCodes.OfficeAdminEmailInvalid);

                var existingAdmin = await userManager.Users
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(u => u.Email == adminEmail, ct);

                if (existingAdmin is not null)
                    return Result.Fail<Guid>(ErrorsCodes.OfficeAdminEmailExists);

                // -----------------------------
                // Prepare Office
                // -----------------------------
                var officeId = Guid.NewGuid();
                var backendName = $"OFF-{Guid.NewGuid()}";
                var officeCode = $"OFF-{Guid.NewGuid()}";

                var office = new Office
                {
                    Id = officeId,
                    BackendName = backendName,
                    NameAr = request.NameAr,
                    NameEn = request.NameEn,
                    CountryId = request.CountryId,
                    Code = officeCode,
                    SupportedCountries = request.SupportedCountryIds
                        .Select(id => new OfficeSupportedCountry { CountryId = id })
                        .ToList()
                };

                // -----------------------------
                // Save Office FIRST (breaks cycle)
                // -----------------------------
                var addOfficeResult = await officeRepo.AddAsync(office);
                if (addOfficeResult.IsFailed)
                    return Result.Fail<Guid>(addOfficeResult.Errors);

                await unitOfWork.SaveChangesAsync(ct);

                // -----------------------------
                // Create Office Admin AFTER office is saved
                // -----------------------------
                var officeAdminResult =
                    OfficeUser.Register(adminEmail, office.Id, "مدير المكتب", "Office Admin");

                if (officeAdminResult.IsFailed)
                    return Result.Fail<Guid>(officeAdminResult.Errors);

                var officeAdmin = officeAdminResult.Value;

                var createUserResult = await userManager.CreateAsync(officeAdmin);
                if (!createUserResult.Succeeded)
                    return Result.Fail<Guid>(ErrorsCodes.OfficeAdminCreationFailed);

                var roleResult = await userManager.AddToRoleAsync(officeAdmin, nameof(SystemRoleIds.OfficeAdmin));
                if (!roleResult.Succeeded)
                    return Result.Fail<Guid>(ErrorsCodes.OfficeRoleAssignmentFailed);
                
                office.OfficeAdminId = officeAdmin.Id;

                await unitOfWork.SaveChangesAsync(ct);
                
                return Result.Ok(office.Id);
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Fail<Guid>(ex.Message);
        }
    }
}
