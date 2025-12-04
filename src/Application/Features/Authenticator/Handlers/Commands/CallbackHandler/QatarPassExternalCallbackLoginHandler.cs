using System.Security.Claims;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands.CallbackHandler;

public class ConstantQatarPass
{
    public const string Provider = "QatarPass";
    public const string DefaultDisplayName = "Qatar Pass";
    public const string PlaceholderEmailDomain = "@login.local";
}
public sealed class QatarPassExternalCallbackLoginHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IQatarPassClient qatarPassClient
) : BaseExternalCallbackLoginHandler, IRequestHandler<QatarPassExternalCallbackLoginCommand, IResult<AuthResponse>>
{

    public async Task<IResult<AuthResponse>> Handle(QatarPassExternalCallbackLoginCommand request, CancellationToken ct)
    {
        // Guard: provider error
        if (!string.IsNullOrEmpty(request.RemoteError))
            return Result.Fail<AuthResponse>(ErrorsCodes.ExternalLoginError(request.RemoteError));

        // Guard: missing token
        if (string.IsNullOrWhiteSpace(request.Authtoken))
            return Result.Fail<AuthResponse>(ErrorsCodes.ExternalLoginInfoNotFound);

        // 1) Fetch external profile
        var qpResult = await FetchQatarPassDataAsync(request.Authtoken!, ct);
        if (qpResult.IsFailed) return Result.Fail<AuthResponse>(qpResult.Errors);

        var qp = qpResult.Value;
        if (string.IsNullOrWhiteSpace(qp.UserQid))
            return Result.Fail<AuthResponse>("QatarPass: QID is missing.");

        var providerKey = qp.UserQid.Trim();
        var normalizedPhone = NormalizePhone(qp.MobileNumber);

        var placeholderEmail = $"qp{providerKey}{ConstantQatarPass.PlaceholderEmailDomain}";

        // 2) If already linked → issue tokens
        var linked = await userManager.FindByLoginAsync(ConstantQatarPass.Provider, providerKey);
        if (linked is not null)
            return await UpsertClaimsAndIssueAsync(linked, qp, normalizedPhone, ct);

        // *** CHANGE: Try to attach to an existing local account by placeholder email ***
        var candidate = await FindCandidateByEmailAsync(placeholderEmail, ct);
        if (candidate is not null)
        {
            var linkRes = await LinkLoginAsync(candidate, providerKey);
            if (linkRes.IsFailed) return Result.Fail<AuthResponse>(linkRes.Errors);

            return await UpsertClaimsAndIssueAsync(candidate, qp, normalizedPhone, ct);
        }

        // 4) Create + link + enrich + tokens
        var createLinkIssue = await CreateLinkAndIssueAsync(providerKey, normalizedPhone, qp, ct);
        if (createLinkIssue.IsFailed) return Result.Fail<AuthResponse>(createLinkIssue.Errors);

        return Result.Ok(createLinkIssue.Value);
    }

    // -----------------------
    // External data fetch
    // -----------------------
    private async Task<IResult<QatarPassAccount>> FetchQatarPassDataAsync(string authToken, CancellationToken ct)
    {
        var res = await qatarPassClient.GetDataAsync(authToken, ct);
        if (res.IsFailed) return Result.Fail<QatarPassAccount>(res.Errors);

        // Defensive: ensure Account exists and has at least one element
        var account = res.Value?.Account.FirstOrDefault();
        if (account is null)
            return Result.Fail<QatarPassAccount>("QatarPass: Account payload is empty.");

        return Result.Ok(account);
    }

    // -----------------------
    // Users lookup / create
    // -----------------------
    private async Task<User?> FindCandidateByEmailAsync(string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return await userManager.FindByEmailAsync(email);
    }

    private async Task<IResult<AuthResponse>> CreateLinkAndIssueAsync(
        string providerKey,
        string? normalizedPhone,
        QatarPassAccount qp,
        CancellationToken ct)
    {
        var placeholderEmail = $"qp{providerKey}{ConstantQatarPass.PlaceholderEmailDomain}";
        var newUserResult = User.Register(placeholderEmail, ConstantQatarPass.DefaultDisplayName, UserTypeIds.Applicant);
        if (newUserResult.IsFailed) return Result.Fail<AuthResponse>(newUserResult.Errors);

        var newUser = (ApplicantUser)newUserResult.Value;

        if (!string.IsNullOrWhiteSpace(normalizedPhone))
        {
            newUser.PhoneNumber = normalizedPhone;
            newUser.PhoneNumberConfirmed = true;
        }

        var createRes = await userManager.CreateAsync(newUser);
        if (!createRes.Succeeded) return FailureFromIdentity<AuthResponse>(createRes);

        var linkRes = await LinkLoginAsync(newUser, providerKey);
        if (linkRes.IsFailed) return Result.Fail<AuthResponse>(linkRes.Errors);

        return await UpsertClaimsAndIssueAsync(newUser, qp, normalizedPhone, ct);
    }

    private async Task<Result> LinkLoginAsync(User user, string providerKey)
    {
        var addLogin = await userManager.AddLoginAsync(user, new UserLoginInfo(ConstantQatarPass.Provider, providerKey, ConstantQatarPass.Provider));
        return addLogin.Succeeded
            ? Result.Ok()
            : Result.Fail(string.Join(", ", addLogin.Errors.Select(e => e.Description)));
    }

    // -----------------------
    // Claims + tokens
    // -----------------------
    private async Task<IResult<AuthResponse>> UpsertClaimsAndIssueAsync(
        User user,
        QatarPassAccount qp,
        string? normalizedPhone,
        CancellationToken ct)
    {
        var upsert = await UpsertQatarPassClaimsAsync(user, qp, normalizedPhone);
        if (upsert.IsFailed) return Result.Fail<AuthResponse>(upsert.Errors);
        return await tokenService.IssueTokensAsync(user, ct);
    }

    private async Task<Result> UpsertQatarPassClaimsAsync(User user, QatarPassAccount data, string? normalizedPhone)
    {
        var existing = await userManager.GetClaimsAsync(user);

        // Map of claim suffix -> value
        var claims = new (string Key, string? Value)[]
        {
            ("qid", data.UserQid),
            ("mobile", normalizedPhone),
            ("nationality", data.Nationality),
            ("accountType", data.AccountType),
            ("accountSubType", data.AccountSubType),
            ("code", data.Code),
            ("accessTokenExpiration", data.AccessTokenExpiration)
        };

        foreach (var (key, value) in claims)
        {
            var type = $"qatarpass:{key}";
            var current = existing.FirstOrDefault(c => c.Type == type);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (current is not null)
                    await userManager.RemoveClaimAsync(user, current);
                continue;
            }

            var next = new Claim(type, value);

            if (current is null)
                await userManager.AddClaimAsync(user, next);
            else if (current.Value != value)
                await userManager.ReplaceClaimAsync(user, current, next);
        }

        user.PhoneNumberConfirmed = true;
        var update = await userManager.UpdateAsync(user);
        return update.Succeeded ? Result.Ok() : FailureFromIdentity(update);
    }

    // -----------------------
    // Helpers
    // -----------------------
    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        // Qatar mobiles commonly include 974; we normalize to +974XXXXXXXX
        if (digits.StartsWith("974") && digits.Length == 11) return "+" + digits; // +974XXXXXXXX
        if (digits.Length == 8) return "+974" + digits;
        return string.IsNullOrWhiteSpace(digits) ? null : "+" + digits;
    }

    private static Result<T> FailureFromIdentity<T>(IdentityResult res) =>
        Result.Fail<T>(string.Join(", ", res.Errors.Select(e => e.Description)));

    private static Result FailureFromIdentity(IdentityResult res) =>
        Result.Fail(string.Join(", ", res.Errors.Select(e => e.Description)));
}
