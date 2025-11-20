using System.Security.Claims;
using CSharpFunctionalExtensions;
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

public sealed class QatarPassExternalCallbackLoginHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IQatarPassClient qatarPassClient
) : BaseExternalCallbackLoginHandler, IRequestHandler<QatarPassExternalCallbackLoginCommand, Result<AuthResponse>>
{
    private const string Provider = "QatarPass";
    private const string DefaultDisplayName = "Qatar Pass User";
    private const string PlaceholderEmailDomain = "@login.local";

    public async Task<Result<AuthResponse>> Handle(QatarPassExternalCallbackLoginCommand request, CancellationToken ct)
    {
        // Guard: provider error
        if (!string.IsNullOrEmpty(request.RemoteError))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginError(request.RemoteError));

        // Guard: missing token
        if (string.IsNullOrWhiteSpace(request.Authtoken))
            return Result.Failure<AuthResponse>(ErrorsCodes.ExternalLoginInfoNotFound);

        // 1) Fetch external profile
        var qpResult = await FetchQatarPassDataAsync(request.Authtoken!, ct);
        if (qpResult.IsFailure) return Result.Failure<AuthResponse>(qpResult.Error);

        var qp = qpResult.Value;
        if (string.IsNullOrWhiteSpace(qp.UserQid))
            return Result.Failure<AuthResponse>("QatarPass: QID is missing.");

        var providerKey = qp.UserQid.Trim();
        var normalizedPhone = NormalizePhone(qp.MobileNumber);

        var placeholderEmail = $"qp{providerKey}{PlaceholderEmailDomain}";

        // 2) If already linked → issue tokens
        var linked = await userManager.FindByLoginAsync(Provider, providerKey);
        if (linked is not null)
            return await UpsertClaimsAndIssueAsync(linked, qp, normalizedPhone, ct);

        // *** CHANGE: Try to attach to an existing local account by placeholder email ***
        var candidate = await FindCandidateByEmailAsync(placeholderEmail, ct);
        if (candidate is not null)
        {
            var linkRes = await LinkLoginAsync(candidate, providerKey);
            if (linkRes.IsFailure) return Result.Failure<AuthResponse>(linkRes.Error);

            return await UpsertClaimsAndIssueAsync(candidate, qp, normalizedPhone, ct);
        }

        // 4) Create + link + enrich + tokens
        var createLinkIssue = await CreateLinkAndIssueAsync(providerKey, normalizedPhone, qp, ct);
        if (createLinkIssue.IsFailure) return Result.Failure<AuthResponse>(createLinkIssue.Error);

        return createLinkIssue.Value;
    }

    // -----------------------
    // External data fetch
    // -----------------------
    private async Task<Result<QatarPassAccount>> FetchQatarPassDataAsync(string authToken, CancellationToken ct)
    {
        var res = await qatarPassClient.GetDataAsync(authToken, ct);
        if (res.IsFailure) return Result.Failure<QatarPassAccount>(res.Error);

        // Defensive: ensure Account exists and has at least one element
        var account = res.Value?.Account.FirstOrDefault();
        if (account is null)
            return Result.Failure<QatarPassAccount>("QatarPass: Account payload is empty.");

        return account;
    }

    // -----------------------
    // Users lookup / create
    // -----------------------
    private async Task<User?> FindCandidateByEmailAsync(string email, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return await userManager.FindByEmailAsync(email);
    }

    private async Task<Result<AuthResponse>> CreateLinkAndIssueAsync(
        string providerKey,
        string? normalizedPhone,
        QatarPassAccount qp,
        CancellationToken ct)
    {
        var placeholderEmail = $"qp{providerKey}{PlaceholderEmailDomain}";
        var newUserResult = User.Register(placeholderEmail, DefaultDisplayName, UserTypeIds.Applicant);
        if (newUserResult.IsFailure) return Result.Failure<AuthResponse>(newUserResult.Error);

        var newUser = (ApplicantUser)newUserResult.Value;

        if (!string.IsNullOrWhiteSpace(normalizedPhone))
        {
            newUser.PhoneNumber = normalizedPhone;
            newUser.PhoneNumberConfirmed = true;
        }

        var createRes = await userManager.CreateAsync(newUser);
        if (!createRes.Succeeded) return FailureFromIdentity<AuthResponse>(createRes);

        var linkRes = await LinkLoginAsync(newUser, providerKey);
        if (linkRes.IsFailure) return Result.Failure<AuthResponse>(linkRes.Error);

        return await UpsertClaimsAndIssueAsync(newUser, qp, normalizedPhone, ct);
    }

    private async Task<Result> LinkLoginAsync(User user, string providerKey)
    {
        var addLogin = await userManager.AddLoginAsync(user, new UserLoginInfo(Provider, providerKey, Provider));
        return addLogin.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join(", ", addLogin.Errors.Select(e => e.Description)));
    }

    // -----------------------
    // Claims + tokens
    // -----------------------
    private async Task<Result<AuthResponse>> UpsertClaimsAndIssueAsync(
        User user,
        QatarPassAccount qp,
        string? normalizedPhone,
        CancellationToken ct)
    {
        var upsert = await UpsertQatarPassClaimsAsync(user, qp, normalizedPhone);
        if (upsert.IsFailure) return Result.Failure<AuthResponse>(upsert.Error);
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

        user.EmailConfirmed = true;
        var update = await userManager.UpdateAsync(user);
        return update.Succeeded ? Result.Success() : FailureFromIdentity(update);
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
        Result.Failure<T>(string.Join(", ", res.Errors.Select(e => e.Description)));

    private static Result FailureFromIdentity(IdentityResult res) =>
        Result.Failure(string.Join(", ", res.Errors.Select(e => e.Description)));
}
