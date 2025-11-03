using System.Net.Http.Json;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;

public sealed class QatarPassExternalCallbackLoginHandler(
    IUnitOfWork uow,
    UserManager<ApplicantUser> userManager,
    ITokenService tokenService,
    IHttpClientFactory factory
) : BaseExternalCallbackLoginHandler, IRequestHandler<QatarPassExternalCallbackLoginCommand, Result<AuthResponse>>
{
    private const string Provider = "QatarPass";
    private const string DefaultDisplayName = "Qatar Pass User";

    public async Task<Result<AuthResponse>> Handle(QatarPassExternalCallbackLoginCommand request, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(request.RemoteError))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginError(request.RemoteError));

        if (string.IsNullOrWhiteSpace(request.Authtoken))
            return Result.Failure<AuthResponse>(ErrorsCodes
                .ExternalLoginInfoNotFound); // or define QATAR_PASS_TOKEN_REQUIRED

        // 1) Call Qatar Pass data endpoint
        var qp = await FetchQatarPassDataAsync(request.Authtoken, ct);
        if (qp.IsFailure) return Result.Failure<AuthResponse>(qp.Error);

        var data = qp.Value;
        if (string.IsNullOrWhiteSpace(data.UserQid))
            return Result.Failure<AuthResponse>("QatarPass: QID is missing.");

        var providerKey = data.UserQid.Trim();

        // 2) Already linked?
        var linkedUser = await userManager.FindByLoginAsync(Provider, providerKey);
        if (linkedUser != null)
        {
            await UpsertQatarPassClaimsAsync(userManager, linkedUser, data);
            return await IssueTokensAsync(linkedUser, userManager, tokenService, uow, ct);
        }

        // 3) Try to attach to an existing local account (heuristics)
        //    a) by normalized phone (if you trust it to be unique)
        ApplicantUser? candidate = null;
        var normalizedPhone = NormalizePhone(data.MobileNumber);
        if (!string.IsNullOrWhiteSpace(normalizedPhone))
        {
            candidate = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == normalizedPhone, ct);
        }

        if (candidate != null)
        {
            var linkRes =
                await userManager.AddLoginAsync(candidate, new UserLoginInfo(Provider, providerKey, Provider));
            if (!linkRes.Succeeded)
                return Result.Failure<AuthResponse>(string.Join(", ", linkRes.Errors.Select(e => e.Description)));

            await UpsertQatarPassClaimsAsync(userManager, candidate, data);
            return await IssueTokensAsync(candidate, userManager, tokenService, uow, ct);
        }

        // 4) Create a new local user and link
        // Use a safe placeholder email that will never collide with real domains
        var placeholderEmail = $"qp{providerKey}@login.local";
        var newUserResult = User.Register(placeholderEmail, DefaultDisplayName, UserTypeIds.Applicant);
        if (newUserResult.IsFailure)
            return Result.Failure<AuthResponse>(newUserResult.Error);

        var newUser = (ApplicantUser)newUserResult.Value;

        // Optional enrichments if your User has these fields:
        if (!string.IsNullOrWhiteSpace(normalizedPhone))
        {
            newUser.PhoneNumber = normalizedPhone;
            newUser.PhoneNumberConfirmed = true;
        }

        var createRes = await userManager.CreateAsync(newUser);
        if (!createRes.Succeeded)
            return Result.Failure<AuthResponse>(string.Join(", ", createRes.Errors.Select(e => e.Description)));

        var addLogin = await userManager.AddLoginAsync(newUser, new UserLoginInfo(Provider, providerKey, Provider));
        if (!addLogin.Succeeded)
            return Result.Failure<AuthResponse>(string.Join(", ", addLogin.Errors.Select(e => e.Description)));

        await UpsertQatarPassClaimsAsync(userManager, newUser, data);
        return await IssueTokensAsync(newUser, userManager, tokenService, uow, ct);
    }

    // --- External call ---
    private async Task<Result<QatarPassAccount>> FetchQatarPassDataAsync(string authtoken, CancellationToken ct)
    {
        var client = factory.CreateClient("QatarPass");
        using var res = await client.GetAsync($"api/Services/GetData?Code={authtoken}", ct);
        if (!res.IsSuccessStatusCode)
            return Result.Failure<QatarPassAccount>($"QatarPass API error: {(int)res.StatusCode}");

        var payload = await res.Content.ReadFromJsonAsync<QatarPassEnvelope>(cancellationToken: ct);
        if (payload is null || payload.IsSuccess != true || payload.ResponseData is null ||
            payload.ResponseData.Count == 0)
            return Result.Failure<QatarPassAccount>("QatarPass: empty/invalid response.");

        return Result.Success(payload.ResponseData[0]);
    }
    
    private static async Task UpsertQatarPassClaimsAsync(UserManager<ApplicantUser> userManager, ApplicantUser user, QatarPassAccount data)
    {
        var existing = await userManager.GetClaimsAsync(user);

        async Task Upsert(string type, string? value)
        {
            var claimType = $"qatarpass:{type}";
            var old = existing.FirstOrDefault(c => c.Type == claimType);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (old != null) await userManager.RemoveClaimAsync(user, old);
                return;
            }

            var @new = new Claim(claimType, value);
            if (old == null) await userManager.AddClaimAsync(user, @new);
            else if (old.Value != value) await userManager.ReplaceClaimAsync(user, old, @new);
        }

        await Upsert("qid", data.UserQid);
        await Upsert("mobile", NormalizePhone(data.MobileNumber));
        await Upsert("nationality", data.Nationality);
        await Upsert("accountType", data.AccountType);
        await Upsert("accountSubType", data.AccountSubType);
        await Upsert("code", data.Code);
        await Upsert("accessTokenExpiration", data.AccessTokenExpiration);
        
        // user.NationalId = data.UserQid;
        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);
    }

    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;
        var s = new string(phone.Where(char.IsDigit).ToArray());
        // Qatar mobile often like "+9745xxxxxxx" → keep as digits with country code
        if (s.StartsWith("974") && s.Length == 11) return "+" + s; // +974XXXXXXXX
        if (s.Length == 8) return "+974" + s;
        return string.IsNullOrWhiteSpace(s) ? null : "+" + s;
    }
}
