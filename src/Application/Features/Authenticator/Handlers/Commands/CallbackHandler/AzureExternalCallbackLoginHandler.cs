using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;

public sealed class AzureExternalCallbackLoginHandler(
    IExternalIdTokenValidator azureTokenValidator,
    IUnitOfWork uow,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService
) : BaseExternalCallbackLoginHandler, IRequestHandler<AzureExternalCallbackLoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(AzureExternalCallbackLoginCommand request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Error))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginError(request.Error));
        if (string.IsNullOrWhiteSpace(request.IdToken))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginInfoNotFound);

        var principalResult = await azureTokenValidator.ValidateAsync(request.IdToken, ct);
        if (principalResult.IsFailure)
            return principalResult.ConvertFailure<AuthResponse>();

        var principal = principalResult.Value;

        // Extract claims
        var provider = "Azure";
        var providerKey = GetProviderKey(principal);

        if (string.IsNullOrWhiteSpace(providerKey))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginMissingProviderKey);

        var email = principal.FindFirst(ClaimTypes.Email)?.Value ??
                    principal.FindFirst("preferred_username")?.Value ??
                    principal.FindFirst("emails")?.Value;

        var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value
                        ?? principal.FindFirst("given_name")?.Value;

        var surname   = principal.FindFirst(ClaimTypes.Surname)?.Value
                        ?? principal.FindFirst("family_name")?.Value;

        var fullName  = principal.FindFirst(ClaimTypes.Name)?.Value ??
                        principal.FindFirst("name")?.Value ??
                        $"{principal.FindFirst(ClaimTypes.GivenName)?.Value} {principal.FindFirst(ClaimTypes.Surname)?.Value}".Trim();

        // If already linked, sign-in directly
        var linkedUser = await userManager.FindByLoginAsync(provider, providerKey);
        if (linkedUser is not null)
        {
            await UpsertProviderClaimsAsync(userManager, linkedUser, provider, principal);
            await signInManager.SignInAsync(linkedUser, isPersistent: false);
            return await IssueTokensAsync(linkedUser, userManager, tokenService, uow, ct);
        }

        // Not linked: attach to existing by email, or create new
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

            
            return await IssueTokensAsync(existingUser, userManager, tokenService, uow, ct);
        }

        // Create user from claims
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

        var createUserRes = User.Register(email, $"{givenName} {surname}".Trim(), UserTypeIds.Employee);
        if (createUserRes.IsFailure)
            return Result.Failure<AuthResponse>(createUserRes.Error);

        var newUser = (EmployeeUser)createUserRes.Value;
        var createRes = await userManager.CreateAsync(newUser);
        if (!createRes.Succeeded)
            return Result.Failure<AuthResponse>(string.Join(", ", createRes.Errors.Select(e => e.Description)));

        var addLogin = await userManager.AddLoginAsync(newUser,
            new UserLoginInfo(provider, providerKey, "Azure AD"));
        if (!addLogin.Succeeded)
            return Result.Failure<AuthResponse>(string.Join(", ", addLogin.Errors.Select(e => e.Description)));

        await UpsertProviderClaimsAsync(userManager, newUser, provider, principal);
        return await IssueTokensAsync(newUser, userManager, tokenService, uow, ct);
    }
    private static string? GetProviderKey(ClaimsPrincipal p)
    {
        // Prefer Azure AD object id (oid). When mapping is ON it shows as the URI.
        var oid =
            p.FindFirst("oid")?.Value ??
            p.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value ??
            p.FindFirst(ClaimTypes.NameIdentifier)?.Value; // last-ditch

        // For multi-tenant apps, pair with tid to be truly unique across tenants
        var tid = p.FindFirst("tid")?.Value ??
                  p.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

        if (!string.IsNullOrWhiteSpace(oid) && !string.IsNullOrWhiteSpace(tid))
            return $"{tid}:{oid}"; // stable across tenants

        // Fall back to sub (works for MSA/personal accounts too)
        return oid ?? p.FindFirst("sub")?.Value;
    }
    private static async Task UpsertProviderClaimsAsync(UserManager<User> userManager, User user, string provider, ClaimsPrincipal principal)
    {
        var email        = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirst("preferred_username")?.Value;
        var givenName    = principal.FindFirstValue(ClaimTypes.GivenName) ?? principal.FindFirst("given_name")?.Value;
        var surname      = principal.FindFirstValue(ClaimTypes.Surname)   ?? principal.FindFirst("family_name")?.Value;
        var fullName     = principal.FindFirstValue(ClaimTypes.Name)      ?? principal.FindFirst("name")?.Value;
        var picture      = principal.FindFirst("picture")?.Value;
        var profile      = principal.FindFirst("profile")?.Value;
        var locale       = principal.FindFirst("locale")?.Value;

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
        user.EmailConfirmed = true;

        await userManager.UpdateAsync(user);
    }
}
