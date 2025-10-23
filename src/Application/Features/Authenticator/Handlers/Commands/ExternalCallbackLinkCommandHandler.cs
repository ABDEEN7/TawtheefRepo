using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class ExternalCallbackLinkCommandHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager
) : IRequestHandler<ExternalCallbackLinkCommand, Result<SocialAccounts>>
{
    public async Task<Result<SocialAccounts>> Handle(ExternalCallbackLinkCommand request, CancellationToken ct)
    {
        if (request.RemoteError is not null)
            return Result.Failure<SocialAccounts>(ErrorsCodes.ExternalLoginError(request.RemoteError));

        var user = await userManager.FindByIdAsync($"{request.UserId}");
        if (user is null) return Result.Failure<SocialAccounts>(ErrorsCodes.ExternalLoginUserNotFound);

        // IMPORTANT: pass userId anti-forgery key (same value used in ConfigureExternalAuthenticationProperties)
        var info = await signInManager.GetExternalLoginInfoAsync($"{request.UserId}");
        if (info is null) return Result.Failure<SocialAccounts>(ErrorsCodes.ExternalLoginInfoNotFound);

        // Block linking if this provider identity belongs to another user
        var linkedTo = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (linkedTo is not null && linkedTo.Id != user.Id)
            return Result.Failure<SocialAccounts>(ErrorsCodes.ExternalLoginAlreadyLinked);

        // Prevent duplicate provider for the same user (e.g., re-link)
        var existingLogins = await userManager.GetLoginsAsync(user);
        var existingProviderLogin = existingLogins.FirstOrDefault(l => l.LoginProvider == info.LoginProvider);
        if (existingProviderLogin is not null)
            return Result.Failure<SocialAccounts>(ErrorsCodes.ExternalLoginProviderAlreadyLinked);

        // Link it
        var add = await userManager.AddLoginAsync(user, info);
        if (!add.Succeeded)
            return Result.Failure<SocialAccounts>(string.Join(", ", add.Errors.Select(e => e.Description)));

        // OPTIONAL: persist the external tokens (when available)
        await signInManager.UpdateExternalAuthenticationTokensAsync(info);

        // Capture & store useful claims (safe, UX-friendly)
        await UpsertProviderClaimsAsync(userManager, user, info);

        return Result.Success(new SocialAccounts(info, user.Email));
    }

    private static async Task UpsertProviderClaimsAsync(UserManager<User> userManager, User user, ExternalLoginInfo info)
    {
        // Extract common claims
        var email       = info.Principal.FindFirstValue(ClaimTypes.Email);
        var givenName   = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var surname     = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var fullName    = info.Principal.FindFirstValue(ClaimTypes.Name);
        var picture     = info.Principal.FindFirst("picture")?.Value;
        var profile     = info.Principal.FindFirst("profile")?.Value;
        var locale      = info.Principal.FindFirst("locale")?.Value;
        var emailVerStr = info.Principal.FindFirst("email_verified")?.Value;
        var emailVerified = string.Equals(emailVerStr, "true", StringComparison.OrdinalIgnoreCase);

        // Hydrate user properties only if missing (avoid overwriting user edits)
        if (string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(givenName)) user.FirstName = givenName;
        if (string.IsNullOrWhiteSpace(user.LastName)  && !string.IsNullOrWhiteSpace(surname))   user.LastName  = surname;

        // Fallback: split Name to first/last if needed
        if ((string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName)) && !string.IsNullOrWhiteSpace(fullName))
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrWhiteSpace(user.FirstName) && parts.Length >= 1) user.FirstName = parts[0];
            if (string.IsNullOrWhiteSpace(user.LastName)  && parts.Length >= 2) user.LastName  = string.Join(' ', parts.Skip(1));
        }

        if (string.IsNullOrWhiteSpace(user.Avatar) && !string.IsNullOrWhiteSpace(picture))
            user.Avatar = picture;

        // Store provider-specific claims into AspNetUserClaims using a namespaced key
        // e.g., "google:email", "google:picture", etc.
        var providerKey = info.LoginProvider.ToLowerInvariant();
        var existingClaims = await userManager.GetClaimsAsync(user);
        async Task Upsert(string type, string? value)
        {
            var t = $"{providerKey}:{type}";
            var old = existingClaims.FirstOrDefault(c => c.Type == t);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (old != null) await userManager.RemoveClaimAsync(user, old);
                return;
            }
            var newClaim = new Claim(t, value);
            if (old == null) await userManager.AddClaimAsync(user, newClaim);
            else if (old.Value != value)
            {
                await userManager.ReplaceClaimAsync(user, old, newClaim);
            }
        }

        await Upsert("email", email);
        await Upsert("name", fullName ?? $"{givenName} {surname}".Trim());
        await Upsert("picture", picture);
        await Upsert("profile", profile);
        await Upsert("locale", locale);
        await Upsert("email_verified", emailVerified ? "true" : "false");

        // If the provider asserts verified email AND your app considers that acceptable, you may mark EmailConfirmed.
        if (!user.EmailConfirmed && emailVerified && string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            user.EmailConfirmed = true;
        }

        await userManager.UpdateAsync(user);
    }
}
