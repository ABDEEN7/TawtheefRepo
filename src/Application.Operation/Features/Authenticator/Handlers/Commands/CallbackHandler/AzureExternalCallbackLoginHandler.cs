using System.Security.Claims;
using Application.Operation.Features.Authenticator.Commands;
using Application.Operation.Features.Authenticator.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.Register;

namespace Application.Operation.Features.Authenticator.Handlers.Commands.CallbackHandler;

public sealed class AzureExternalCallbackLoginHandler(
    IExternalIdTokenValidator azureTokenValidator,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService,
    IEmployeeProfileService employeeProfileService,
    ILoginAuditService loginAudit,
    IAppLogger logger
) : BaseExternalCallbackLoginHandler(loginAudit),
    ICommandHandler<AzureExternalCallbackLoginCommand, IResult<AuthResponse>>
{
    private const string ProviderName = "Azure";
    private const string ProviderDisplayName = "Azure AD";

    private readonly IAppLogger _log = logger.ForContext(typeof(AzureExternalCallbackLoginHandler));

    protected override string Provider => ProviderName;
    protected override Guid? DefaultUserType => UserTypeIds.Employee;

    public async Task<IResult<AuthResponse>> Handle(AzureExternalCallbackLoginCommand request, CancellationToken ct)
    {
        _log.Information(
            "Azure callback started. ErrorPresent={ErrorPresent}",
            !string.IsNullOrWhiteSpace(request.Error));

        // 1) Fail fast on external provider error
        if (!string.IsNullOrWhiteSpace(request.Error))
        {
            _log.Warning("Azure callback failed: ProviderError={ProviderError}", request.Error);
            return await LogFailureAsync(ErrorsCodes.ExternalLoginError(request.Error), ct: ct);
        }

        // 2) Get id_token + validate
        var idToken = await GetExternalIdTokenAsync();
        if (idToken is null)
        {
            _log.Warning("Azure callback failed: id_token not found in ExternalLoginInfo tokens");
            return await LogFailureAsync(ErrorsCodes.ExternalLoginInfoNotFound, ct: ct);
        }

        var principalResult = await azureTokenValidator.ValidateAsync(idToken, ct);
        if (principalResult.IsFailed)
        {
            _log.Warning(
                "Azure token validation failed. Errors={Errors}",
                string.Join(" | ", principalResult.Errors.Select(e => e.Message)));

            return await LogFailureAsync(principalResult.Errors, ct: ct);
        }

        var principal = principalResult.Value;

        // 3) Build normalized claims model
        var claims = AzureClaims.From(principal);

        _log.Information(
            "Azure claims extracted. ProviderKeyPresent={ProviderKeyPresent} EmailPresent={EmailPresent}",
            !string.IsNullOrWhiteSpace(claims.ProviderKey),
            !string.IsNullOrWhiteSpace(claims.Email));

        if (string.IsNullOrWhiteSpace(claims.ProviderKey))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginMissingProviderKey, ct: ct);

        if (string.IsNullOrWhiteSpace(claims.Email))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailNotFound, ct: ct);

        if (!IsEduGovQaEmail(claims.Email))
        {
            _log.Warning("Azure callback blocked: email domain not allowed. Email={Email}", claims.Email);
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailDomainNotAllowed, ct: ct);
        }

        // 4) Resolve user (linked -> existing by email -> create new)
        var resolvedUser = await ResolveUserAsync(claims);
        if (resolvedUser.IsFailed)
        {
            _log.Warning(
                "Azure resolve user failed. Email={Email} Errors={Errors}",
                claims.Email,
                string.Join(" | ", resolvedUser.Errors.Select(e => e.Message)));

            return await LogFailureAsync(resolvedUser.Errors, ct: ct);
        }

        var user = resolvedUser.Value;

        _log.Information(
            "Azure user resolved. UserId={UserId} UserTypeId={UserTypeId}",
            user.Id,
            user.UserTypeId);

        // 5) Upsert provider claims + sync employee profile (if applicable)
        await UpsertProviderClaimsAsync(userManager, user, Provider, principal);

        var syncResult = await SyncEmployeeProfileAsync(user, ct);
        if (syncResult.IsFailed)
        {
            _log.Warning(
                "Azure employee profile sync failed. UserId={UserId} Errors={Errors}",
                user.Id,
                string.Join(" | ", syncResult.Errors.Select(e => e.Message)));

            return await LogFailureAsync(syncResult.Errors, user.Id, user.UserTypeId, ct: ct);
        }

        // 6) Sign-in + issue tokens
        await signInManager.SignInAsync(user, isPersistent: false);

        var tokenResult = await tokenService.IssueTokensAsync(user, Provider, ct);

        if (tokenResult.IsFailed)
        {
            _log.Warning(
                "Azure token issuance failed. UserId={UserId} Errors={Errors}",
                user.Id,
                string.Join(" | ", tokenResult.Errors.Select(e => e.Message)));
        }
        else
        {
            _log.Information("Azure login succeeded. UserId={UserId}", user.Id);
        }

        return tokenResult;
    }

    private async Task<string?> GetExternalIdTokenAsync()
    {
        var info = await signInManager.GetExternalLoginInfoAsync();
        return info?.AuthenticationTokens?.FirstOrDefault(t => t.Name == "id_token")?.Value;
    }

    private async Task<Result<User>> ResolveUserAsync(AzureClaims claims)
    {
        // A) If already linked, return that user
        var linkedUser = await userManager.FindByLoginAsync(Provider, claims.ProviderKey);
        if (linkedUser is not null)
        {
            _log.Information("Azure resolve: already linked. UserId={UserId}", linkedUser.Id);
            return Result.Ok(linkedUser);
        }

        // B) If user exists by email, link this provider to that account
        var existingUser = await userManager.FindByEmailAsync(claims.Email);
        if (existingUser is not null)
        {
            _log.Information("Azure resolve: found by email; linking provider. UserId={UserId}", existingUser.Id);
            return await LinkProviderToExistingUserAsync(existingUser, claims.ProviderKey);
        }

        // C) Otherwise create a new employee user
        _log.Information("Azure resolve: user not found; creating new employee. Email={Email}", claims.Email);
        return await CreateEmployeeUserAsync(claims);

        async Task<Result<User>> LinkProviderToExistingUserAsync(User user, string providerKey)
        {
            // Defensive: ensure providerKey isn't already linked to another account
            var duplicate = await userManager.FindByLoginAsync(Provider, providerKey);
            if (duplicate is not null && duplicate.Id != user.Id)
            {
                _log.Warning(
                    "Azure resolve: provider key already linked to different user. ExistingUserId={ExistingUserId} DuplicateUserId={DuplicateUserId}",
                    user.Id,
                    duplicate.Id);

                return Result.Fail(ErrorsCodes.ExternalLoginAlreadyLinked);
            }

            var addLoginRes = await userManager.AddLoginAsync(
                user,
                new UserLoginInfo(Provider, providerKey, ProviderDisplayName));

            if (!addLoginRes.Succeeded)
            {
                var errors = string.Join(", ", addLoginRes.Errors.Select(e => e.Description));
                _log.Warning("Azure resolve: AddLoginAsync failed. UserId={UserId} Errors={Errors}", user.Id, errors);
                return Result.Fail(errors);
            }

            return Result.Ok(user);
        }
    }

    private async Task<Result<User>> CreateEmployeeUserAsync(AzureClaims claims)
    {
        var (given, surname) = claims.GetBestEffortNameParts();

        var registerResult = User.Register(
            claims.Email,
            $"{given} {surname}".Trim(),
            UserTypeIds.Employee);

        if (registerResult.IsFailed)
        {
            _log.Warning(
                "Azure create employee failed: domain register failed. Email={Email} Errors={Errors}",
                claims.Email,
                string.Join(" | ", registerResult.Errors.Select(e => e.Message)));

            return Result.Fail(registerResult.Errors);
        }

        var newUser = (EmployeeUser)registerResult.Value;

        newUser.AddDomainEvent(new EmployeeRegisterEvent(
            newUser.Id,
            newUser.FullNameEn,
            newUser.Email!,
            DateTimeOffset.UtcNow));

        var createRes = await userManager.CreateAsync(newUser);
        if (!createRes.Succeeded)
        {
            var errors = string.Join(", ", createRes.Errors.Select(e => e.Description));
            _log.Warning("Azure create employee failed: CreateAsync failed. Email={Email} Errors={Errors}", claims.Email, errors);
            return Result.Fail(errors);
        }

        var addLoginRes = await userManager.AddLoginAsync(
            newUser,
            new UserLoginInfo(Provider, claims.ProviderKey, ProviderDisplayName));

        if (!addLoginRes.Succeeded)
        {
            var errors = string.Join(", ", addLoginRes.Errors.Select(e => e.Description));
            _log.Warning("Azure create employee failed: AddLoginAsync failed. UserId={UserId} Errors={Errors}", newUser.Id, errors);
            return Result.Fail(errors);
        }

        _log.Information("Azure create employee succeeded. UserId={UserId}", newUser.Id);

        return Result.Ok<User>(newUser);
    }

    private async Task<Result> SyncEmployeeProfileAsync(User user, CancellationToken ct)
    {
        if (user is AdminUser)
            return Result.Ok();

        if (user is not EmployeeUser employeeUser)
            return Result.Fail(ErrorsCodes.ExternalLoginOfficeUserInvalidType);

        await employeeProfileService.SyncFromDirectoryAsync(employeeUser, ct);
        return Result.Ok();
    }

    private static bool IsEduGovQaEmail(string? email)
        => !string.IsNullOrWhiteSpace(email)
           && email.EndsWith("@edu.gov.qa", StringComparison.OrdinalIgnoreCase);

    private static async Task UpsertProviderClaimsAsync(
        UserManager<User> userManager,
        User user,
        string provider,
        ClaimsPrincipal principal)
    {
        var data = ProviderProfileClaims.FromAzure(principal);

        // Populate missing core fields (domain model)
        if (string.IsNullOrWhiteSpace(user.FullNameEn))
            user.FullNameEn = data.FullName ?? data.Email!.Split('@')[0];
        if (string.IsNullOrWhiteSpace(user.FullNameAr))
            user.FullNameAr = data.FullName ?? data.Email!.Split('@')[0];

        user.Avatar ??= data.Picture;
        user.EmailConfirmed = true;

        var existingClaims = await userManager.GetClaimsAsync(user);
        var prefix = provider.ToLowerInvariant();

        async Task Upsert(string type, string? value)
        {
            var key = $"{prefix}:{type}";
            var old = existingClaims.FirstOrDefault(c => c.Type == key);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (old != null) await userManager.RemoveClaimAsync(user, old);
                return;
            }

            var newer = new Claim(key, value);
            if (old == null) await userManager.AddClaimAsync(user, newer);
            else if (!string.Equals(old.Value, value, StringComparison.Ordinal))
                await userManager.ReplaceClaimAsync(user, old, newer);
        }

        await Upsert("email", data.Email);
        await Upsert("name", data.FullName);
        await Upsert("picture", data.Picture);
        await Upsert("profile", data.Profile);
        await Upsert("locale", data.Locale);

        await userManager.UpdateAsync(user);
    }
}
