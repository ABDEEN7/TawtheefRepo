using Application.Operation.Common.Interfaces.Services.HttpClients;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Infrastructure.Services.Identity;

public class EmployeeProfileService(
    IEmployeeDirectoryClient directoryClient,
    UserManager<User> userManager,
    IAppLogger logger)
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

        var profileChanged = UpsertEmployeeProfile(user, profile);
        var userChanged = SynchronizeUserFields(user, profile);

        if (!profileChanged && !userChanged)
            return Result.Ok(profile);

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = updateResult.Errors
                .Select(x => x.Description)
                .ToArray();

            logger.Error(
                "Failed to persist HR employee profile. UserId={UserId} Errors={Errors}",
                user.Id,
                string.Join(" | ", errors));

            return Result.Fail<EmployeeProfileInfo>(errors);
        }

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
    /// Synchronizes authoritative HR names and fills the phone number when it is missing.
    /// Returns true if any field changed.
    /// </summary>
    private bool SynchronizeUserFields(EmployeeUser user, EmployeeProfileInfo profile)
    {
        var changed = false;

        changed |= SynchronizeName(
            profile.FullNameEn,
            user.FullNameEn,
            value => user.FullNameEn = value,
            nameof(User.FullNameEn),
            user.Id);

        changed |= SynchronizeName(
            profile.FullNameAr,
            user.FullNameAr,
            value => user.FullNameAr = value,
            nameof(User.FullNameAr),
            user.Id);

        if (IsEmpty(user.PhoneNumber) && IsNotEmpty(profile.MobileNumber))
        {
            user.PhoneNumber = profile.MobileNumber;
            changed = true;
        }

        return changed;
    }

    private bool SynchronizeName(
        string? hrValue,
        string currentValue,
        Action<string> assign,
        string fieldName,
        Guid userId)
    {
        const int employeNameMaxLength = 100;

        if (string.IsNullOrWhiteSpace(hrValue) ||
            string.Equals(currentValue, hrValue, StringComparison.Ordinal))
            return false;

        if (hrValue.Length > employeNameMaxLength)
        {
            logger.Warning(
                "HR employee name exceeds the supported length and was not synchronized. UserId={UserId} Field={Field} Length={Length} MaxLength={MaxLength}",
                userId,
                fieldName,
                hrValue.Length,
                employeNameMaxLength);
            return false;
        }

        assign(hrValue);
        return true;
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

