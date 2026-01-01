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
using Tawtheef.Domain.Events.Operation.Employee.Register;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;

public sealed class AzureExternalCallbackLoginHandler(
    IExternalIdTokenValidator azureTokenValidator,
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    ITokenService tokenService,
    ILoginAuditService loginAudit//,
    //IEmployeeProfileService employeeProfileService
) : BaseExternalCallbackLoginHandler(loginAudit), IRequestHandler<AzureExternalCallbackLoginCommand, IResult<AuthResponse>>
{
    protected override string Provider => "Azure";
    protected override Guid? DefaultUserType => UserTypeIds.Employee;
    public async Task<IResult<AuthResponse>> Handle(AzureExternalCallbackLoginCommand request, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(request.Error))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginError(request.Error), ct: ct);
        var info = await signInManager.GetExternalLoginInfoAsync();
        var idToken = info?.AuthenticationTokens?.FirstOrDefault(t => t.Name == "id_token")?.Value;
        if (idToken is null)
            return await LogFailureAsync(ErrorsCodes.ExternalLoginInfoNotFound, ct: ct);
        var principalResult = await azureTokenValidator.ValidateAsync(idToken, ct);
        if (principalResult.IsFailed)
            return await LogFailureAsync(principalResult.Errors, ct: ct);

        var principal = principalResult.Value;

        // Extract claims
        var providerKey = GetProviderKey(principal);

        if (string.IsNullOrWhiteSpace(providerKey))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginMissingProviderKey, ct: ct);

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
        if (string.IsNullOrWhiteSpace(email))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailNotFound, ct: ct);

        if (!IsEduGovQaEmail(email))
            return await LogFailureAsync(ErrorsCodes.ExternalLoginEmailDomainNotAllowed, ct: ct);
        // If already linked, sign-in directly
        var linkedUser = await userManager.FindByLoginAsync(Provider, providerKey);
        if (linkedUser is not null)
        {
            await UpsertProviderClaimsAsync(userManager, linkedUser, Provider, principal);
            var syncResult = await SyncEmployeeProfileAsync(linkedUser, ct);
            if (syncResult.IsFailed)
                return await LogFailureAsync(syncResult.Errors, linkedUser.Id, linkedUser.UserTypeId, ct: ct);
            await signInManager.SignInAsync(linkedUser, isPersistent: false);
            return await tokenService.IssueTokensAsync(linkedUser, Provider, ct);
        }

        var existingUser = await userManager.FindByEmailAsync(email);
        if (existingUser is not null)
        {
            // Guard against duplicate link to another account
            var duplicate = await userManager.FindByLoginAsync(Provider, providerKey);
            if (duplicate is not null && duplicate.Id != existingUser.Id)
                return await LogFailureAsync(ErrorsCodes.ExternalLoginAlreadyLinked, existingUser.Id, existingUser.UserTypeId, ct: ct);

            var addLoginRes = await userManager.AddLoginAsync(existingUser,
                new UserLoginInfo(Provider, providerKey, "Azure AD"));
            if (!addLoginRes.Succeeded)
                return await LogFailureAsync(string.Join(", ", addLoginRes.Errors.Select(e => e.Description)),
                    existingUser.Id, existingUser.UserTypeId, ct: ct);

            await UpsertProviderClaimsAsync(userManager, existingUser, Provider, principal);
            var syncExistingResult = await SyncEmployeeProfileAsync(existingUser, ct);
            if (syncExistingResult.IsFailed)
                return await LogFailureAsync(syncExistingResult.Errors, existingUser.Id, existingUser.UserTypeId, ct: ct);
            await signInManager.SignInAsync(existingUser, isPersistent: false);


            return await tokenService.IssueTokensAsync(existingUser, Provider, ct);
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
        if (createUserRes.IsFailed)
            return await LogFailureAsync(createUserRes.Errors, ct: ct);

        var newUser = (EmployeeUser)createUserRes.Value;
        newUser.AddDomainEvent(new EmployeeRegisterEvent(newUser.Id, newUser.FullNameEn, newUser.Email!, DateTimeOffset.UtcNow));
        
        var createRes = await userManager.CreateAsync(newUser);
        if (!createRes.Succeeded)
            return await LogFailureAsync(string.Join(", ", createRes.Errors.Select(e => e.Description)), newUser.Id, newUser.UserTypeId, ct: ct);

        var addLogin = await userManager.AddLoginAsync(newUser,
            new UserLoginInfo(Provider, providerKey, "Azure AD"));
        if (!addLogin.Succeeded)
            return await LogFailureAsync(string.Join(", ", addLogin.Errors.Select(e => e.Description)), newUser.Id, newUser.UserTypeId, ct: ct);

        await UpsertProviderClaimsAsync(userManager, newUser, Provider, principal);
        
        var syncNewResult = await SyncEmployeeProfileAsync(newUser, ct);
        if (syncNewResult.IsFailed)
            return await LogFailureAsync(syncNewResult.Errors, newUser.Id, newUser.UserTypeId, ct: ct);
        return await tokenService.IssueTokensAsync(newUser, Provider, ct);
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

        if(string.IsNullOrEmpty(user.FullNameEn) && !string.IsNullOrEmpty(fullName))
            user.FullNameEn = fullName;
        
        if(string.IsNullOrEmpty(user.FullNameAr) && !string.IsNullOrEmpty(fullName))
            user.FullNameAr = fullName;

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

    private async Task<Result> SyncEmployeeProfileAsync(User user, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
        return Result.Ok();
        
        // if (user is not EmployeeUser employeeUser)
        //     return Result.Fail(ErrorsCodes.ExternalLoginOfficeUserInvalidType);
        //
        // var syncResult = await employeeProfileService.SyncFromDirectoryAsync(employeeUser, ct);
        // return syncResult.IsFailed
        //     ? Result.Fail(syncResult.Errors)
        //     : Result.Ok();
    }

    private static bool IsEduGovQaEmail(string? email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
               email.EndsWith("@edu.gov.qa", StringComparison.OrdinalIgnoreCase);
    }
}
