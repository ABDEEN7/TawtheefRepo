using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;

public class GoogleExternalCallbackLoginHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService,
    ILoginAuditService loginAudit
) : BaseExternalCallbackLoginHandler, IRequestHandler<GoogleExternalCallbackLoginCommand, IResult<AuthResponse>>
{
    public async Task<IResult<AuthResponse>> Handle(GoogleExternalCallbackLoginCommand request, CancellationToken cancellationToken)
    {
        const string provider = "Google";
        var defaultUserType = UserTypeIds.Applicant;

        async Task<IResult<AuthResponse>> LogFailureAsync(string reason, Guid? userId = null, Guid? userTypeId = null)
        {
            await loginAudit.LogAsync(new LoginAttemptEntry(userId, userTypeId ?? defaultUserType, provider, false, reason),
                cancellationToken);
            return Result.Fail<AuthResponse>(reason);
        }

        async Task<IResult<AuthResponse>> LogFailureAsync(IEnumerable<IError> errors, Guid? userId = null, Guid? userTypeId = null)
        {
            var reason = string.Join(", ", errors.Select(e => e.Message));
            await loginAudit.LogAsync(new LoginAttemptEntry(userId, userTypeId ?? defaultUserType, provider, false, reason),
                cancellationToken);
            return Result.Fail<AuthResponse>(errors);
        }

        if (request.RemoteError != null)
            return await LogFailureAsync(ErrorsCodes.ExternalLoginError(request.RemoteError));

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return await LogFailureAsync(ErrorsCodes.ExternalLoginInfoNotFound);

        // If already linked, sign in directly
        var result = await signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            var linkedUser = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (linkedUser == null)
                return await LogFailureAsync(ErrorsCodes.ExternalLoginUserNotFound, linkedUser?.Id, linkedUser?.UserTypeId);

            // Update tokens from provider & upsert claims
            await signInManager.UpdateExternalAuthenticationTokensAsync(info);
            await UpsertProviderClaimsAsync(userManager, linkedUser, info);

            return await tokenService.IssueTokensAsync(linkedUser, provider, cancellationToken);
        }

        // Not linked yet: use email to attach or create a new user
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailNotFound);

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            var duplicate = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (duplicate != null && duplicate.Id != existingUser.Id)
                return await LogFailureAsync(ErrorsCodes.ExternalLoginAlreadyLinked, existingUser.Id, existingUser.UserTypeId);

            var addLoginResult = await userManager.AddLoginAsync(existingUser, info);
            if (!addLoginResult.Succeeded)
                return await LogFailureAsync(string.Join(", ", addLoginResult.Errors.Select(e => e.Description)),
                    existingUser.Id, existingUser.UserTypeId);

            // Update provider tokens & claims snapshot
            await signInManager.UpdateExternalAuthenticationTokensAsync(info);
            await UpsertProviderClaimsAsync(userManager, existingUser, info);

            await signInManager.SignInAsync(existingUser, isPersistent: false);
            
            return await tokenService.IssueTokensAsync(existingUser, provider, cancellationToken);
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

        var newUserResult = User.Register(email,$"{givenName} {surname}".Trim(), UserTypeIds.Applicant);
        if(newUserResult.IsFailed)
            return await LogFailureAsync(newUserResult.Errors);
        
        var newUser = (ApplicantUser)newUserResult.Value;
        var createResult = await userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
            return await LogFailureAsync(string.Join(", ", createResult.Errors.Select(e => e.Description)), newUser.Id, newUser.UserTypeId);

        var addLogin = await userManager.AddLoginAsync(newUser, info);
        if (!addLogin.Succeeded)
        {
            var errors = addLogin.Errors.Select(e => e.Description);
            return await LogFailureAsync(string.Join(", ", errors), newUser.Id, newUser.UserTypeId);
        }

        // Save tokens & claims
        await signInManager.UpdateExternalAuthenticationTokensAsync(info);
        await UpsertProviderClaimsAsync(userManager, newUser, info);

        return await tokenService.IssueTokensAsync(newUser, provider, cancellationToken);
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

        if(string.IsNullOrEmpty(user.FullNameEn) && !string.IsNullOrEmpty(fullName))
            user.FullNameEn = fullName;
        
        if(string.IsNullOrEmpty(user.FullNameAr) && !string.IsNullOrEmpty(fullName))
            user.FullNameAr = fullName;

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
        user.EmailConfirmed = true;

        await userManager.UpdateAsync(user);
    }
}
