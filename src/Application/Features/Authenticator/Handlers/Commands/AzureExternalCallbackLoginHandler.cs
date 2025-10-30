using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public sealed class AzureExternalCallbackLoginHandler(
    IExternalIdTokenValidator azureTokenValidator, // bound to Azure impl
    IUnitOfWork uow,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService
) : IRequestHandler<AzureExternalCallbackLoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(AzureExternalCallbackLoginCommand request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.RemoteError))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginError(request.RemoteError));

        // 1) Validate Azure ID token → ClaimsPrincipal
        var principalResult = await azureTokenValidator.ValidateAsync(request.IdToken, ct);
        if (principalResult.IsFailure)
            return principalResult.ConvertFailure<AuthResponse>();

        var principal = principalResult.Value;

        // 2) Extract claims
        var provider = NormalizeProvider(request.Provider);               // "Azure"
        var providerKey = principal.FindFirst("sub")?.Value
                          ?? principal.FindFirst("oid")?.Value;

        if (string.IsNullOrWhiteSpace(providerKey))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginMissingProviderKey);

        var email = principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? principal.FindFirst("preferred_username")?.Value;

        var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value
                        ?? principal.FindFirst("given_name")?.Value;

        var surname   = principal.FindFirst(ClaimTypes.Surname)?.Value
                        ?? principal.FindFirst("family_name")?.Value;

        var fullName  = principal.FindFirst(ClaimTypes.Name)?.Value
                        ?? principal.FindFirst("name")?.Value;

        // 3) If already linked, sign-in directly
        var linkedUser = await userManager.FindByLoginAsync(provider, providerKey);
        if (linkedUser is not null)
        {
            await UpsertProviderClaimsAsync(userManager, linkedUser, provider, principal);

            // No ExternalLoginInfo here; just sign-in cookie if you rely on it
            await signInManager.SignInAsync(linkedUser, isPersistent: false);

            // Load required nav (e.g., UserType) if you display it
            if (linkedUser.UserTypeId != default)
            {
                linkedUser.UserType = await uow.GetEntityRepository<UserType>().DbSet
                    .FirstAsync(t => t.Id == linkedUser.UserTypeId, ct);
            }

            var access  = tokenService.GenerateAccessToken(linkedUser);
            var refresh = tokenService.GenerateRefreshToken(linkedUser.Id);

            return Result.Success(new AuthResponse(
                new UserInfoResponse(linkedUser.Id, linkedUser.GivenNameEn, linkedUser.FamilyNameEn, linkedUser.Email!, linkedUser.Avatar),
                new TokenResponse(access.Token, access.Expires, refresh.Token, refresh.Expires)
            ));
        }

        // 4) Not linked: attach to existing by email, or create new
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginEmailNotFound);

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            // Guard against duplicate link to another account
            var duplicate = await userManager.FindByLoginAsync(provider, providerKey);
            if (duplicate is not null && duplicate.Id != existingUser.Id)
                return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginAlreadyLinked);

            var addLoginRes = await userManager.AddLoginAsync(existingUser,
                new UserLoginInfo(provider, providerKey, "Azure AD"));
            if (!addLoginRes.Succeeded)
                return Result.Failure<AuthResponse>(string.Join(", ", addLoginRes.Errors.Select(e => e.Description)));

            await UpsertProviderClaimsAsync(userManager, existingUser, provider, principal);
            await signInManager.SignInAsync(existingUser, isPersistent: false);

            var access  = tokenService.GenerateAccessToken(existingUser);
            var refresh = tokenService.GenerateRefreshToken(existingUser.Id);

            return Result.Success(new AuthResponse(
                new UserInfoResponse(existingUser.Id, existingUser.GivenNameEn, existingUser.FamilyNameEn, existingUser.Email!, existingUser.Avatar),
                new TokenResponse(access.Token, access.Expires, refresh.Token, refresh.Expires)
            ));
        }

        // 5) Create user from claims
        if (string.IsNullOrWhiteSpace(givenName) || string.IsNullOrWhiteSpace(surname))
        {
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                givenName ??= parts.FirstOrDefault() ?? "User";
                surname   ??= (parts.Length > 1) ? string.Join(' ', parts.Skip(1)) : "Account";
            }
            else
            {
                givenName ??= "User";
                surname   ??= "Account";
            }
        }

        var createUserRes = User.Register(email, $"{givenName} {surname}".Trim(), nameof(UserTypeIds.Applicant));
        if (createUserRes.IsFailure)
            return Result.Failure<AuthResponse>(createUserRes.Error);

        var newUser = createUserRes.Value;
        var createRes = await userManager.CreateAsync(newUser);
        if (!createRes.Succeeded)
            return Result.Failure<AuthResponse>(string.Join(", ", createRes.Errors.Select(e => e.Description)));

        var addLogin = await userManager.AddLoginAsync(newUser,
            new UserLoginInfo(provider, providerKey, "Azure AD"));
        if (!addLogin.Succeeded)
            return Result.Failure<AuthResponse>(string.Join(", ", addLogin.Errors.Select(e => e.Description)));

        await UpsertProviderClaimsAsync(userManager, newUser, provider, principal);

        // Your policy: require admin approval for brand-new external accounts
        return Result.Failure<AuthResponse>(ErrorsCodes.YourAccountRequiresAdminApproval);
    }

    private static string NormalizeProvider(string provider)
        => provider.Trim().ToLowerInvariant() switch
        {
            "azure" or "azuread" or "microsoft" => "Azure",
            _ => provider
        };

    // Mirrors your Google UpsertProviderClaimsAsync but works with ClaimsPrincipal
    private static async Task UpsertProviderClaimsAsync(UserManager<User> userManager, User user, string provider, ClaimsPrincipal principal)
    {
        var email        = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirst("preferred_username")?.Value;
        var givenName    = principal.FindFirstValue(ClaimTypes.GivenName) ?? principal.FindFirst("given_name")?.Value;
        var surname      = principal.FindFirstValue(ClaimTypes.Surname)   ?? principal.FindFirst("family_name")?.Value;
        var fullName     = principal.FindFirstValue(ClaimTypes.Name)      ?? principal.FindFirst("name")?.Value;
        var picture      = principal.FindFirst("picture")?.Value;     // may not exist in Azure
        var profile      = principal.FindFirst("profile")?.Value;     // may not exist in Azure
        var locale       = principal.FindFirst("locale")?.Value;      // may not exist in Azure
        var emailVerStr  = principal.FindFirst("email_verified")?.Value
                           ?? principal.FindFirst("emails:verified")?.Value;
        var emailVerified = string.Equals(emailVerStr, "true", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(user.GivenNameEn) && !string.IsNullOrWhiteSpace(givenName)) user.GivenNameEn = givenName;
        if (string.IsNullOrWhiteSpace(user.FamilyNameEn) && !string.IsNullOrWhiteSpace(surname))  user.FamilyNameEn = surname;

        if ((string.IsNullOrWhiteSpace(user.GivenNameEn) || string.IsNullOrWhiteSpace(user.FamilyNameEn)) && !string.IsNullOrWhiteSpace(fullName))
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrWhiteSpace(user.GivenNameEn)  && parts.Length >= 1) user.GivenNameEn  = parts[0];
            if (string.IsNullOrWhiteSpace(user.FamilyNameEn) && parts.Length >= 2) user.FamilyNameEn = string.Join(' ', parts.Skip(1));
        }

        if (string.IsNullOrWhiteSpace(user.Avatar) && !string.IsNullOrWhiteSpace(picture))
            user.Avatar = picture;

        var existingClaims = await userManager.GetClaimsAsync(user);
        var prefix = provider.ToLowerInvariant(); // "azure"

        async Task Upsert(string type, string? value)
        {
            var t = $"{prefix}:{type}";
            var old = existingClaims.FirstOrDefault(c => c.Type == t);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (old != null) await userManager.RemoveClaimAsync(user, old);
                return;
            }
            var newer = new Claim(t, value);
            if (old == null) await userManager.AddClaimAsync(user, newer);
            else if (old.Value != value) await userManager.ReplaceClaimAsync(user, old, newer);
        }

        await Upsert("email", email);
        await Upsert("name", fullName ?? $"{givenName} {surname}".Trim());
        await Upsert("picture", picture);
        await Upsert("profile", profile);
        await Upsert("locale", locale);
        await Upsert("email_verified", emailVerified ? "true" : "false");

        if (!user.EmailConfirmed && emailVerified && !string.IsNullOrWhiteSpace(email) &&
            string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            user.EmailConfirmed = true;
        }

        await userManager.UpdateAsync(user);
    }
}
