using Application.Operation.Common.Interfaces.Services.HttpClients;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Identity;

public class EmployeeProfileService(
    IEmployeeDirectoryClient directoryClient,
    UserManager<User> userManager)
    : IEmployeeProfileService
{
    public async Task<IResult<EmployeeProfileInfo>> SyncFromDirectoryAsync(
        EmployeeUser user,
        CancellationToken ct = default)
    {
        var validationResult = ValidateUserEmail(user);
        if (validationResult.IsFailed)
            return Result.Fail<EmployeeProfileInfo>(validationResult.Errors);

        var profileResult = await directoryClient.GetByEmailAsync(user.Email!, ct);
        if (profileResult.IsFailed)
            return Result.Fail<EmployeeProfileInfo>(profileResult.Errors);

        var profile = profileResult.Value;
        var needsUpdate =
            UpsertEmployeeProfile(user, profile) |
            FillMissingUserFields(user, profile);

        if (needsUpdate)
            await userManager.UpdateAsync(user);

        return Result.Ok(profile);
    }

    private static IResult<Unit> ValidateUserEmail(EmployeeUser user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            return Result.Fail<Unit>(ErrorsCodes.ExternalLoginEmailNotFound);

        if (!user.Email.EndsWith("@edu.gov.qa", StringComparison.OrdinalIgnoreCase) 
            // && !user.Email.EndsWith("@education.qa", StringComparison.OrdinalIgnoreCase)
            )
            return Result.Fail<Unit>(ErrorsCodes.ExternalLoginEmailDomainNotAllowed);

        return Result.Ok(Unit.Value);
    }

    /// <summary>
    /// Updates the user primitive fields ONLY if they are currently empty.
    /// Returns true if any field changed.
    /// </summary>
    private static bool FillMissingUserFields(EmployeeUser user, EmployeeProfileInfo profile)
    {
        var changed = false;

        if (IsEmpty(user.FullNameEn) && IsNotEmpty(profile.FullNameEn))
        {
            user.FullNameEn = profile.FullNameEn;
            changed = true;
        }

        if (IsEmpty(user.FullNameAr) && IsNotEmpty(profile.FullNameAr))
        {
            user.FullNameAr = profile.FullNameAr;
            changed = true;
        }

        if (IsEmpty(user.PhoneNumber) && IsNotEmpty(profile.MobileNumber))
        {
            user.PhoneNumber = profile.MobileNumber;
            changed = true;
        }

        return changed;
    }

    private static bool IsEmpty(string? value)
        => string.IsNullOrWhiteSpace(value);

    private static bool IsNotEmpty(string? value)
        => !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Replaces EmployeeProfile if payload differs (or doesn't exist).
    /// Returns true if profile was updated.
    /// </summary>
    private static bool UpsertEmployeeProfile(EmployeeUser user, EmployeeProfileInfo profile)
    {
        if (HasSamePayload(user.EmployeeProfile, profile))
            return false;

        user.EmployeeProfile = Map(profile);
        return true;
    }

    private static bool HasSamePayload(EmployeeProfile? existing, EmployeeProfileInfo incoming)
        => existing is not null &&
           string.Equals(existing.RawPayload, incoming.RawPayload, StringComparison.Ordinal);

    private static EmployeeProfile Map(EmployeeProfileInfo p) => new()
    {
        EmployeeNumber = p.EmployeeNumber,
        Qid = p.Qid,
        FullNameAr = p.FullNameAr,
        FullNameEn = p.FullNameEn,
        Email = p.Email,
        MobileNumber = p.MobileNumber,
        Department = p.Department,
        DepartmentNumber = p.DepartmentNumber,
        Section = p.Section,
        SectionNumber = p.SectionNumber,
        JobTitle = p.JobTitle,
        RawPayload = p.RawPayload
    };
}

