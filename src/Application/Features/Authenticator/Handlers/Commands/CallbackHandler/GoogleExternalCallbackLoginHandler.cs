using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;

public class GoogleExternalCallbackLoginHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService,
    IUnitOfWork uow,
    ILoginAuditService loginAudit
) : BaseExternalCallbackLoginHandler(loginAudit),
    IRequestHandler<GoogleExternalCallbackLoginCommand, IResult<AuthResponse>>
{
    protected override string Provider => "Google";
    protected override Guid? DefaultUserType => null;

    public async Task<IResult<AuthResponse>> Handle(
        GoogleExternalCallbackLoginCommand request,
        CancellationToken ct)
    {
        if (request.RemoteError is not null)
            return await LogFailureAsync(ErrorsCodes.ExternalLoginError(request.RemoteError), ct: ct);

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
            return await LogFailureAsync(ErrorsCodes.ExternalLoginInfoNotFound, ct: ct);

        // 1) If already linked => sign in and issue tokens
        var linkedSignIn = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (linkedSignIn.Succeeded)
            return await HandleAlreadyLinkedAsync(request, info, ct);

        // 2) Not linked yet => branch by requested user type
        return request.DefaultUserType == UserTypeIds.OfficeUser
            ? await HandleOfficeNotLinkedAsync(info, ct)
            : await HandleApplicantNotLinkedAsync(info, ct);
    }

    private async Task<IResult<AuthResponse>> HandleAlreadyLinkedAsync(
        GoogleExternalCallbackLoginCommand request,
        ExternalLoginInfo info,
        CancellationToken ct)
    {
        var linkedUser = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (linkedUser is null)
            return await LogFailureAsync(ErrorsCodes.ExternalLoginUserNotFound, ct: ct);

        if (request.DefaultUserType == UserTypeIds.OfficeUser)
        {
            var officeCheck = await EnsureActiveOfficeUserAsync(linkedUser, ct);
            if (officeCheck.IsFailed)
                return await LogFailureAsync(officeCheck.Errors, linkedUser.Id, linkedUser.UserTypeId, ct: ct);
        }

        return await FinalizeLoginAsync(linkedUser, info, ct);
    }

    private async Task<IResult<AuthResponse>> HandleOfficeNotLinkedAsync(ExternalLoginInfo info, CancellationToken ct)
    {
        var email = GetEmail(info);
        if (string.IsNullOrWhiteSpace(email))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailNotFound, ct: ct);

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is null)
            return await LogFailureAsync(ErrorsCodes.ExternalLoginNotLinkedOfficeUser, ct: ct);

        var officeCheck = await EnsureActiveOfficeUserAsync(existingUser, ct);
        if (officeCheck.IsFailed)
            return await LogFailureAsync(officeCheck.Errors, existingUser.Id, existingUser.UserTypeId, ct: ct);

        // Prevent providerKey being linked to a different user
        var providerAlreadyLinked = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (providerAlreadyLinked is not null && providerAlreadyLinked.Id != existingUser.Id)
            return await LogFailureAsync(
                ErrorsCodes.ExternalLoginAlreadyLinked,
                existingUser.Id, existingUser.UserTypeId,
                ct: ct);

        var addLogin = await userManager.AddLoginAsync(existingUser, info);
        if (!addLogin.Succeeded)
            return await LogFailureAsync(
                JoinIdentityErrors(addLogin),
                existingUser.Id, existingUser.UserTypeId,
                ct: ct);

        await signInManager.SignInAsync(existingUser, isPersistent: false);
        return await FinalizeLoginAsync(existingUser, info, ct);
    }

    private async Task<IResult<AuthResponse>> HandleApplicantNotLinkedAsync(ExternalLoginInfo info, CancellationToken ct)
    {
        var email = GetEmail(info);
        if (string.IsNullOrWhiteSpace(email))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailNotFound, ct: ct);

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
            return await AttachProviderToExistingApplicantAsync(existingUser, info, ct);

        var newApplicantResult = CreateApplicantFromClaims(info, email);
        if (newApplicantResult.IsFailed)
            return await LogFailureAsync(newApplicantResult.Errors, ct: ct);

        var newApplicant = newApplicantResult.Value;
        var create = await userManager.CreateAsync(newApplicant);
        if (!create.Succeeded)
            return await LogFailureAsync(JoinIdentityErrors(create), newApplicant.Id, newApplicant.UserTypeId, ct: ct);

        var addLogin = await userManager.AddLoginAsync(newApplicant, info);
        if (!addLogin.Succeeded)
            return await LogFailureAsync(JoinIdentityErrors(addLogin), newApplicant.Id, newApplicant.UserTypeId, ct: ct);

        await signInManager.SignInAsync(newApplicant, isPersistent: false);
        return await FinalizeLoginAsync(newApplicant, info, ct);
    }

    private async Task<IResult<AuthResponse>> AttachProviderToExistingApplicantAsync(
        User existingUser,
        ExternalLoginInfo info,
        CancellationToken ct)
    {
        var providerAlreadyLinked = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (providerAlreadyLinked is not null && providerAlreadyLinked.Id != existingUser.Id)
            return await LogFailureAsync(
                ErrorsCodes.ExternalLoginAlreadyLinked,
                existingUser.Id, existingUser.UserTypeId,
                ct: ct);

        var addLogin = await userManager.AddLoginAsync(existingUser, info);
        if (!addLogin.Succeeded)
            return await LogFailureAsync(
                JoinIdentityErrors(addLogin),
                existingUser.Id, existingUser.UserTypeId,
                ct: ct);

        await signInManager.SignInAsync(existingUser, isPersistent: false);
        return await FinalizeLoginAsync(existingUser, info, ct);
    }

    private async Task<IResult<AuthResponse>> FinalizeLoginAsync(User user, ExternalLoginInfo info, CancellationToken ct)
    {
        await signInManager.UpdateExternalAuthenticationTokensAsync(info);
        await UpsertProviderClaimsAsync(userManager, user, info);
        return await tokenService.IssueTokensAsync(user, Provider, ct);
    }

    private static string? GetEmail(ExternalLoginInfo info)
        => info.Principal.FindFirstValue(ClaimTypes.Email);

    private static string JoinIdentityErrors(IdentityResult result)
        => string.Join(", ", result.Errors.Select(e => e.Description));

    private static Result<ApplicantUser> CreateApplicantFromClaims(ExternalLoginInfo info, string email)
    {
        var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var surname = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);

        (givenName, surname) = EnsureNames(givenName, surname, fullName);

        var register = User.Register(email, $"{givenName} {surname}".Trim(), UserTypeIds.Applicant);
        if (register.IsFailed)
            return Result.Fail(register.Errors);

        return Result.Ok((ApplicantUser)register.Value);
    }

    private static (string givenName, string surname) EnsureNames(string? givenName, string? surname, string? fullName)
    {
        if (!string.IsNullOrWhiteSpace(givenName) && !string.IsNullOrWhiteSpace(surname))
            return (givenName, surname);

        if (string.IsNullOrWhiteSpace(fullName))
            return (givenName ?? "User", surname ?? "Account");

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var gn = givenName ?? parts.FirstOrDefault() ?? "User";
        var sn = surname ?? (parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : "Account");
        return (gn, sn);

    }

    /// <summary>
    /// Office user must exist, be OfficeUser, have OfficeId, and Office must not be deleted.
    /// Adjust property names (OfficeId/Office.IsDeleted) to your domain model.
    /// </summary>
    private async Task<Result> EnsureActiveOfficeUserAsync(User user, CancellationToken ct)
    {
        if (user.UserTypeId != UserTypeIds.OfficeUser || user is not OfficeUser officeUser)
            return Result.Fail(ErrorsCodes.ExternalLoginOfficeUserInvalidType);

        if (officeUser.OfficeId == Guid.Empty)
            return Result.Fail(ErrorsCodes.ExternalLoginOfficeUserNotLinkedToOffice);

        var officeRepo = uow.GetEntityRepository<Office>();

        var officeExists = await officeRepo.DbSet.AsNoTracking()
            .AnyAsync(o => o.Id == officeUser.OfficeId, ct);

        return officeExists
            ? Result.Ok()
            : Result.Fail(ErrorsCodes.ExternalLoginOfficeUserOfficeDeletedOrNotFound);
    }

    private static async Task UpsertProviderClaimsAsync(UserManager<User> userManager, User user, ExternalLoginInfo info)
    {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var surname = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
        var picture = info.Principal.FindFirst("picture")?.Value;
        var profile = info.Principal.FindFirst("profile")?.Value;
        var locale = info.Principal.FindFirst("locale")?.Value;

        if (string.IsNullOrEmpty(user.FullNameEn) && !string.IsNullOrEmpty(fullName))
            user.FullNameEn = fullName;

        if (string.IsNullOrEmpty(user.FullNameAr) && !string.IsNullOrEmpty(fullName))
            user.FullNameAr = fullName;

        if (string.IsNullOrWhiteSpace(user.Avatar) && !string.IsNullOrWhiteSpace(picture))
            user.Avatar = picture;

        var providerKey = info.LoginProvider.ToLowerInvariant();
        var existingClaims = await userManager.GetClaimsAsync(user);

        async Task Upsert(string type, string? value)
        {
            var claimType = $"{providerKey}:{type}";
            var old = existingClaims.FirstOrDefault(c => c.Type == claimType);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (old != null) await userManager.RemoveClaimAsync(user, old);
                return;
            }

            var @new = new Claim(claimType, value);
            if (old == null) await userManager.AddClaimAsync(user, @new);
            else if (old.Value != value) await userManager.ReplaceClaimAsync(user, old, @new);
        }

        await Upsert("email", email);
        await Upsert("name", fullName ?? $"{givenName} {surname}".Trim());
        await Upsert("picture", picture);
        await Upsert("profile", profile);
        await Upsert("locale", locale);

        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);
    }
}
