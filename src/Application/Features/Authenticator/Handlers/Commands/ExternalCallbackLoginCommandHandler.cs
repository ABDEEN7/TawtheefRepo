using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class ExternalCallbackLoginCommandHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService
) : IRequestHandler<ExternalCallbackLoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(ExternalCallbackLoginCommand request, CancellationToken cancellationToken)
    {
        if (request.RemoteError != null)
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginError(request.RemoteError));

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginInfoNotFound);

        // If already linked, sign in directly
        var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
                return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginUserNotFound);

            // Update tokens from provider & upsert claims
            await signInManager.UpdateExternalAuthenticationTokensAsync(info);
            await UpsertProviderClaimsAsync(userManager, user, info);

            user.UserType = await uow.GetEntityRepository<UserType>().DbSet
                .FirstAsync(t => t.Id == user.UserTypeId, cancellationToken);

            var accessToken  = tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken(user.Id);

            return Result.Success(new AuthResponse(
                new UserInfoResponse(user.Id, user.GivenNameEn, user.FamilyNameEn, user.Email!, user.Avatar),
                new TokenResponse(accessToken.Token, accessToken.Expires, refreshToken.Token, refreshToken.Expires)
            ));
        }

        // Not linked yet: use email to attach or create a new user
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginEmailNotFound);

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            var duplicate = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (duplicate != null && duplicate.Id != existingUser.Id)
                return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginAlreadyLinked);

            var addLoginResult = await userManager.AddLoginAsync(existingUser, info);
            if (!addLoginResult.Succeeded)
                return Result.Failure<AuthResponse>(string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));

            // Update provider tokens & claims snapshot
            await signInManager.UpdateExternalAuthenticationTokensAsync(info);
            await UpsertProviderClaimsAsync(userManager, existingUser, info);

            await signInManager.SignInAsync(existingUser, isPersistent: false);

            var accessToken  = tokenService.GenerateAccessToken(existingUser);
            var refreshToken = tokenService.GenerateRefreshToken(existingUser.Id);

            return Result.Success(new AuthResponse(
                new UserInfoResponse(existingUser.Id, existingUser.GivenNameEn, existingUser.FamilyNameEn, existingUser.Email!, existingUser.Avatar),
                new TokenResponse(accessToken.Token, accessToken.Expires, refreshToken.Token, refreshToken.Expires)
            ));
        }

        // Create new user from claims (names can be missing for Google/AzureAD on later logins)
        var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var surname   = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var fullName  = info.Principal.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrWhiteSpace(givenName) || string.IsNullOrWhiteSpace(surname))
        {
            // best-effort split
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                givenName ??= parts.FirstOrDefault() ?? "User";
                surname   ??= (parts.Length > 1) ? string.Join(' ', parts.Skip(1)) : "Account";
            }
            else
            {
                // as a hard requirement in your code, throw ValidationException if you want to enforce them
                givenName ??= "User";
                surname   ??= "Account";
            }
        }

        var newUserResult = User.Register(email,$"{givenName} {surname}".Trim(), nameof(UserTypeIds.Applicant));
        if(newUserResult.IsFailure)
            return Result.Failure<AuthResponse>(newUserResult.Error);
        
        var newUser = newUserResult.Value;
        var createResult = await userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
            return Result.Failure<AuthResponse>(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        var addLogin = await userManager.AddLoginAsync(newUser, info);
        if (!addLogin.Succeeded)
        {
            var errors = addLogin.Errors.Select(e => e.Description);
            return Result.Failure<AuthResponse>(string.Join(", ", errors));
        }

        // Save tokens & claims
        await signInManager.UpdateExternalAuthenticationTokensAsync(info);
        await UpsertProviderClaimsAsync(userManager, newUser, info);

        // Your policy requires admin approval for new external accounts
        return Result.Failure<AuthResponse>(ErrorsCodes.YourAccountRequiresAdminApproval);
    }

    private static async Task UpsertProviderClaimsAsync(UserManager<User> userManager, User user, ExternalLoginInfo info)
    {
        // Reuse the same helper as in the Link handler
        // You can extract this method to a shared static class if you like.
        var email= info.Principal.FindFirstValue(ClaimTypes.Email);
        var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
        var surname = info.Principal.FindFirstValue(ClaimTypes.Surname);
        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
        var picture = info.Principal.FindFirst("picture")?.Value;
        var profile = info.Principal.FindFirst("profile")?.Value;
        var locale = info.Principal.FindFirst("locale")?.Value;
        var emailVerStr = info.Principal.FindFirst("email_verified")?.Value;
        var emailVerified = string.Equals(emailVerStr, "true", StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(user.GivenNameEn) && !string.IsNullOrWhiteSpace(givenName)) user.GivenNameEn = givenName;
        if (string.IsNullOrWhiteSpace(user.FamilyNameEn)  && !string.IsNullOrWhiteSpace(surname))   user.FamilyNameEn  = surname;

        if ((string.IsNullOrWhiteSpace(user.GivenNameEn) || string.IsNullOrWhiteSpace(user.FamilyNameEn)) && !string.IsNullOrWhiteSpace(fullName))
        {
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrWhiteSpace(user.GivenNameEn) && parts.Length >= 1) user.GivenNameEn = parts[0];
            if (string.IsNullOrWhiteSpace(user.FamilyNameEn)  && parts.Length >= 2) user.FamilyNameEn  = string.Join(' ', parts.Skip(1));
        }

        if (string.IsNullOrWhiteSpace(user.Avatar) && !string.IsNullOrWhiteSpace(picture))
            user.Avatar = picture;

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
            else if (old.Value != value) await userManager.ReplaceClaimAsync(user, old, newClaim);
        }

        await Upsert("email", email);
        await Upsert("name", fullName ?? $"{givenName} {surname}".Trim());
        await Upsert("picture", picture);
        await Upsert("profile", profile);
        await Upsert("locale", locale);
        await Upsert("email_verified", emailVerified ? "true" : "false");

        if (!user.EmailConfirmed && emailVerified && string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            user.EmailConfirmed = true;

        await userManager.UpdateAsync(user);
    }
}
