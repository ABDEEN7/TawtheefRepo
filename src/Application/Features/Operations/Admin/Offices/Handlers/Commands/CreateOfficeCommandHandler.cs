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

public sealed class CreateOfficeCommandHandler(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager)
    : IRequestHandler<CreateOfficeCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateOfficeCommand request,
        CancellationToken cancellationToken)
    {
        return await unitOfWork.ExecuteInTransactionAsync<IResult<Guid>>(
            ct => HandleInternal(request, ct),
            cancellationToken);
    }

    private async Task<IResult<Guid>> HandleInternal(
        CreateOfficeCommand request,
        CancellationToken ct)
    {
        var emailValidation = await ValidateAdminEmailAsync(request.AdminEmail, ct);
        if (emailValidation.IsFailed)
            return Result.Fail<Guid>(emailValidation.Errors);

        var adminEmail = emailValidation.Value;

        var supportedCountries = CreateSupportedCountries(request.SupportedCountryIds);
        if (supportedCountries.Count == 0)
            return Result.Fail<Guid>(ErrorsCodes.OfficeSupportedCountriesRequired);

        var adminResult = await CreateOfficeAdminAsync(adminEmail);
        if (adminResult.IsFailed)
            return Result.Fail<Guid>(adminResult.Errors);
        var user = adminResult.Value;
        var office = CreateOfficeEntity(request, user.Id, supportedCountries);

        var officeRepo = unitOfWork.GetEntityRepository<Office>();
        var addOfficeResult = await officeRepo.AddAsync(office);
        if (addOfficeResult.IsFailed)
            return Result.Fail<Guid>(addOfficeResult.Errors);
        
        var officeAdmin = await userManager.Users.OfType<OfficeUser>().FirstOrDefaultAsync(u => u.Id == office.OfficeAdminId, ct);
        if (officeAdmin is not null)
        {
            officeAdmin.OfficeId = office.Id;
            await userManager.UpdateAsync(officeAdmin);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok(office.Id);
    }
    
    private async Task<Result<string>> ValidateAdminEmailAsync(
        string? adminEmail, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(adminEmail))
            return Result.Fail<string>(ErrorsCodes.OfficeAdminEmailInvalid);

        var email = adminEmail.Trim();

        var exists = await userManager.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email == email, ct);

        if (exists)
            return Result.Fail<string>(ErrorsCodes.OfficeAdminEmailExists);

        return Result.Ok(email);
    }

    private static List<OfficeSupportedCountry> CreateSupportedCountries(
        IEnumerable<Guid> countryIds)
    {
        return countryIds
            .Distinct()
            .Select(id => new OfficeSupportedCountry { CountryId = id })
            .ToList();
    }

    private async Task<Result<User>> CreateOfficeAdminAsync(string email)
    {
        var registerResult = OfficeUser.Register(email, "Office Admin");
        if (registerResult.IsFailed)
            return Result.Fail<User>(registerResult.Errors);

        var user = registerResult.Value;

        var createResult = await userManager.CreateAsync(user);
        if (!createResult.Succeeded)
            return Result.Fail<User>(ErrorsCodes.OfficeAdminCreationFailed);

        var roleResult = await userManager.AddToRoleAsync(
            user,
            nameof(SystemRoleIds.OfficeAdmin));

        if (!roleResult.Succeeded)
            return Result.Fail<User>(ErrorsCodes.OfficeRoleAssignmentFailed);

        return Result.Ok(user);
    }

    private static Office CreateOfficeEntity(
        CreateOfficeCommand request, Guid adminId,
        List<OfficeSupportedCountry> supportedCountries)
    {
        var code = $"OFF-{Guid.NewGuid():N}";

        return new Office
        {
            BackendName = code,
            Code = code,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            CountryId = request.CountryId,
            OfficeAdminId = adminId,
            SupportedCountries = supportedCountries
        };
    }
}

