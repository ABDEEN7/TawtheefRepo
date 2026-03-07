using Application.Operation.Common.Interfaces.Services.Office;
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

public sealed class CreateOfficeCommandHandler(IUnitOfWork unitOfWork, 
    IOfficeUniquenessChecker uniquenessChecker,
    UserManager<User> userManager,
    IOfficeAdminProvisioner adminProvisioner)
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

    private async Task<IResult<Guid>> HandleInternal(CreateOfficeCommand request, CancellationToken ct)
    {
        // 1) Email validation (application policy)
        var email = request.AdminEmail.Trim();
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<Guid>(ErrorsCodes.OfficeAdminEmailInvalid);

        var existingUser = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (existingUser is not null)
        {
            if (existingUser.UserTypeId == UserTypeIds.Applicant)
                return Result.Fail<Guid>(ErrorsCodes.UserIsApplicant);

            return Result.Fail<Guid>(ErrorsCodes.OfficeAdminEmailExists);
        }

        if (await uniquenessChecker.OfficeCountryExistsAsync(request.CountryId, ct))
            return Result.Fail<Guid>(ErrorsCodes.OfficeCountryAlreadyAssigned);

        // 2) Create admin user (infrastructure hidden behind port)
        var adminResult = await adminProvisioner.CreateOfficeAdminAsync(
            email, request.AdminNameAr, request.AdminNameEn, ct);

        if (adminResult.IsFailed)
            return Result.Fail<Guid>(adminResult.Errors);

        var adminUser = adminResult.Value;

        // 3) Supported countries fallback policy (application)
        var supportedIds = new [] {request.CountryId};

        // 4) Create Office aggregate (domain invariants)
        var officeResult = Office.Create(OfficeCode.New(), request.NameAr, request.NameEn,
            request.CountryId, adminUser.Id, request.PhoneCountryCode,
            request.PhoneNumber, supportedIds);

        if (officeResult.IsFailed)
            return Result.Fail<Guid>(officeResult.Errors);

        var office = officeResult.Value;

        // 5) Persist aggregate
        var officeRepo = unitOfWork.GetEntityRepository<Office>();
        var addOfficeResult = await officeRepo.AddAsync(office, ct);
        if (addOfficeResult.IsFailed)
            return Result.Fail<Guid>(addOfficeResult.Errors);

        // 6) Link admin to office
        var linkResult = await adminProvisioner.AssignOfficeAsync(adminUser.Id, office.Id, ct);
        if (linkResult.IsFailed)
            return Result.Fail<Guid>(linkResult.Errors);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok(office.Id);
    }
}

