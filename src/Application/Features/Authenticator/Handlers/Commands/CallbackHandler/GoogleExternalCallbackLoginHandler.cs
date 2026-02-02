using System.Security.Claims;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
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
    ILoginAuditService loginAudit,
    IAppLogger logger
) : BaseExternalCallbackLoginHandler(loginAudit),
    ICommandHandler<GoogleExternalCallbackLoginCommand, IResult<AuthResponse>>
{
    private readonly IAppLogger _log = logger.ForContext(typeof(GoogleExternalCallbackLoginHandler));

    protected override string Provider => "Google";
    protected override Guid? DefaultUserType => null;

    public async Task<IResult<AuthResponse>> Handle(
        GoogleExternalCallbackLoginCommand request,
        CancellationToken ct)
    {
        _log.Information(
            "Google callback started. DefaultUserType={DefaultUserType} RemoteErrorPresent={RemoteErrorPresent}",
            request.DefaultUserType,
            request.RemoteError is not null);

        if (request.RemoteError is not null)
        {
            _log.Warning("Google callback failed: RemoteError={RemoteError}", request.RemoteError);
            return await LogFailureAsync(ErrorsCodes.ExternalLoginError(request.RemoteError), ct: ct);
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            _log.Warning("Google callback failed: ExternalLoginInfo is null");
            return await LogFailureAsync(ErrorsCodes.ExternalLoginInfoNotFound, ct: ct);
        }

        _log.Information(
            "Google external info retrieved. LoginProvider={LoginProvider} ProviderKey={ProviderKey}",
            info.LoginProvider,
            info.ProviderKey);

        // 1) If already linked => sign in and issue tokens
        var linkedSignIn = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (linkedSignIn.Succeeded)
        {
            _log.Information(
                "Google external sign-in succeeded (already linked). ProviderKey={ProviderKey}",
                info.ProviderKey);

            return await HandleAlreadyLinkedAsync(request, info, ct);
        }

        // 2) Not linked yet => branch by requested user type
        _log.Information(
            "Google external sign-in not linked yet. DefaultUserType={DefaultUserType}",
            request.DefaultUserType);

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
        {
            _log.Warning(
                "Already-linked flow failed: user not found by login. LoginProvider={LoginProvider} ProviderKey={ProviderKey}",
                info.LoginProvider,
                info.ProviderKey);

            return await LogFailureAsync(ErrorsCodes.ExternalLoginUserNotFound, ct: ct);
        }

        _log.Information(
            "Already-linked user resolved. UserId={UserId} UserTypeId={UserTypeId}",
            linkedUser.Id,
            linkedUser.UserTypeId);

        if (request.DefaultUserType == UserTypeIds.OfficeUser)
        {
            var officeCheck = await EnsureActiveOfficeUserAsync(linkedUser, ct);
            if (officeCheck.IsFailed)
            {
                _log.Warning(
                    "Office user check failed (already-linked). UserId={UserId} UserTypeId={UserTypeId} Errors={Errors}",
                    linkedUser.Id,
                    linkedUser.UserTypeId,
                    string.Join(" | ", officeCheck.Errors.Select(e => e.Message)));

                return await LogFailureAsync(officeCheck.Errors, linkedUser.Id, linkedUser.UserTypeId, ct: ct);
            }
        }

        return await FinalizeLoginAsync(linkedUser, info, ct);
    }

    private async Task<IResult<AuthResponse>> HandleOfficeNotLinkedAsync(ExternalLoginInfo info, CancellationToken ct)
    {
        var email = GetEmail(info);
        if (string.IsNullOrWhiteSpace(email))
        {
            _log.Warning("Office-not-linked flow failed: email claim missing. ProviderKey={ProviderKey}", info.ProviderKey);
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailNotFound, ct: ct);
        }

        _log.Information("Office-not-linked flow. Email={Email}", email);

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is null)
        {
            _log.Warning("Office-not-linked flow: no user with email. Email={Email}", email);
            return await LogFailureAsync(ErrorsCodes.ExternalLoginNotLinkedOfficeUser, ct: ct);
        }

        var officeCheck = await EnsureActiveOfficeUserAsync(existingUser, ct);
        if (officeCheck.IsFailed)
        {
            _log.Warning(
                "Office-not-linked flow: office validation failed. UserId={UserId} Errors={Errors}",
                existingUser.Id,
                string.Join(" | ", officeCheck.Errors.Select(e => e.Message)));

            return await LogFailureAsync(officeCheck.Errors, existingUser.Id, existingUser.UserTypeId, ct: ct);
        }

        // Prevent providerKey being linked to a different user
        var providerAlreadyLinked = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (providerAlreadyLinked is not null && providerAlreadyLinked.Id != existingUser.Id)
        {
            _log.Warning(
                "Office-not-linked flow blocked: provider already linked to different user. ExistingUserId={ExistingUserId} LinkedUserId={LinkedUserId}",
                existingUser.Id,
                providerAlreadyLinked.Id);

            return await LogFailureAsync(
                ErrorsCodes.ExternalLoginAlreadyLinked,
                existingUser.Id, existingUser.UserTypeId,
                ct: ct);
        }

        var addLogin = await userManager.AddLoginAsync(existingUser, info);
        if (!addLogin.Succeeded)
        {
            _log.Warning(
                "Office-not-linked flow: AddLoginAsync failed. UserId={UserId} Errors={Errors}",
                existingUser.Id,
                JoinIdentityErrors(addLogin));

            return await LogFailureAsync(
                JoinIdentityErrors(addLogin),
                existingUser.Id, existingUser.UserTypeId,
                ct: ct);
        }

        await signInManager.SignInAsync(existingUser, isPersistent: false);

        _log.Information("Office-not-linked flow succeeded: provider attached and user signed in. UserId={UserId}", existingUser.Id);

        return await FinalizeLoginAsync(existingUser, info, ct);
    }

    private async Task<IResult<AuthResponse>> HandleApplicantNotLinkedAsync(ExternalLoginInfo info, CancellationToken ct)
    {
        var email = GetEmail(info);
        if (string.IsNullOrWhiteSpace(email))
        {
            _log.Warning("Applicant-not-linked flow failed: email claim missing. ProviderKey={ProviderKey}", info.ProviderKey);
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailNotFound, ct: ct);
        }

        _log.Information("Applicant-not-linked flow. Email={Email}", email);

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            _log.Information("Applicant-not-linked: user exists by email, attaching provider. UserId={UserId}", existingUser.Id);
            return await AttachProviderToExistingApplicantAsync(existingUser, info, ct);
        }

        var newApplicantResult = CreateApplicantFromClaims(info, email);
        if (newApplicantResult.IsFailed)
        {
            _log.Warning(
                "Applicant-not-linked: CreateApplicantFromClaims failed. Email={Email} Errors={Errors}",
                email,
                string.Join(" | ", newApplicantResult.Errors.Select(e => e.Message)));

            return await LogFailureAsync(newApplicantResult.Errors, ct: ct);
        }

        var newApplicant = newApplicantResult.Value;

        var create = await userManager.CreateAsync(newApplicant);
        if (!create.Succeeded)
        {
            _log.Warning(
                "Applicant-not-linked: CreateAsync failed. Email={Email} Errors={Errors}",
                email,
                JoinIdentityErrors(create));

            return await LogFailureAsync(JoinIdentityErrors(create), newApplicant.Id, newApplicant.UserTypeId, ct: ct);
        }

        var addLogin = await userManager.AddLoginAsync(newApplicant, info);
        if (!addLogin.Succeeded)
        {
            _log.Warning(
                "Applicant-not-linked: AddLoginAsync failed. UserId={UserId} Errors={Errors}",
                newApplicant.Id,
                JoinIdentityErrors(addLogin));

            return await LogFailureAsync(JoinIdentityErrors(addLogin), newApplicant.Id, newApplicant.UserTypeId, ct: ct);
        }

        await signInManager.SignInAsync(newApplicant, isPersistent: false);

        _log.Information("Applicant-not-linked flow succeeded: new applicant created and signed in. UserId={UserId}", newApplicant.Id);

        return await FinalizeLoginAsync(newApplicant, info, ct);
    }

    private async Task<IResult<AuthResponse>> AttachProviderToExistingApplicantAsync(
        User existingUser,
        ExternalLoginInfo info,
        CancellationToken ct)
    {
        // Prevent providerKey being linked to a different user
        var providerAlreadyLinked = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (providerAlreadyLinked is not null && providerAlreadyLinked.Id != existingUser.Id)
        {
            _log.Warning(
                "AttachProvider blocked: provider already linked to different user. ExistingUserId={ExistingUserId} LinkedUserId={LinkedUserId}",
                existingUser.Id,
                providerAlreadyLinked.Id);

            return await LogFailureAsync(
                ErrorsCodes.ExternalLoginAlreadyLinked,
                existingUser.Id, existingUser.UserTypeId,
                ct: ct);
        }

        var addLogin = await userManager.AddLoginAsync(existingUser, info);
        if (!addLogin.Succeeded)
        {
            _log.Warning(
                "AttachProvider failed: AddLoginAsync failed. UserId={UserId} Errors={Errors}",
                existingUser.Id,
                JoinIdentityErrors(addLogin));

            return await LogFailureAsync(
                JoinIdentityErrors(addLogin),
                existingUser.Id, existingUser.UserTypeId,
                ct: ct);
        }

        await signInManager.SignInAsync(existingUser, isPersistent: false);

        _log.Information("AttachProvider succeeded: user signed in. UserId={UserId}", existingUser.Id);

        return await FinalizeLoginAsync(existingUser, info, ct);
    }

    private async Task<IResult<AuthResponse>> FinalizeLoginAsync(User user, ExternalLoginInfo info, CancellationToken ct)
    {
        _log.Information(
            "FinalizeLogin started. UserId={UserId} LoginProvider={LoginProvider}",
            user.Id,
            info.LoginProvider);

        await signInManager.UpdateExternalAuthenticationTokensAsync(info);
        await UpsertProviderClaimsAsync(userManager, user, info);

        var result = await tokenService.IssueTokensAsync(user, Provider, ct);

        if (result.IsFailed)
        {
            _log.Warning(
                "FinalizeLogin failed: token service returned failure. UserId={UserId} Errors={Errors}",
                user.Id,
                string.Join(" | ", result.Errors.Select(e => e.Message)));
        }
        else
        {
            _log.Information("FinalizeLogin succeeded. UserId={UserId}", user.Id);
        }

        return result;
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
