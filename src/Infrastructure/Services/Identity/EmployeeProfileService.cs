using Application.Operation.Common.Interfaces.Services.HttpClients;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Infrastructure.Data;

namespace Tawtheef.Infrastructure.Services.Identity;

public class EmployeeProfileService(
    IEmployeeDirectoryClient directoryClient,
    UserManager<User> userManager,
    TawtheefDbContext dbContext,
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

        var normalizedQid = QidUtilities.Normalize(profileResult.Value.Qid);
        if (!QidUtilities.IsValid(normalizedQid))
        {
            logger.Warning(
                "HR employee profile synchronization rejected an invalid QID. UserId={UserId}",
                user.Id);
            return Result.Fail<EmployeeProfileInfo>(ErrorsCodes.EmployeeDirectoryInvalidPayload);
        }

        var profile = profileResult.Value with { Qid = normalizedQid };

        var profileSyncResult = await SynchronizeEmployeeProfileAsync(user, profile, ct);
        if (profileSyncResult.IsFailed)
            return Result.Fail<EmployeeProfileInfo>(profileSyncResult.Errors);

        var profileChanged = profileSyncResult.Value;
        var userChanged = SynchronizeUserFields(user, profile);

        if (!profileChanged && !userChanged)
            return Result.Ok(profile);

        IdentityResult updateResult;
        try
        {
            updateResult = await userManager.UpdateAsync(user);
        }
        catch (DbUpdateException ex) when (IsEmployeeProfileUniqueConstraintViolation(ex))
        {
            return await RecoverFromConcurrentProfileCreationAsync(user, profile, ct);
        }

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

        if (!user.Email.EndsWith("@edu.gov.qa", StringComparison.OrdinalIgnoreCase))
            return Result.Fail<Unit>(ErrorsCodes.ExternalLoginEmailDomainNotAllowed);

        return Result.Ok(Unit.Value);
    }

    /// <summary>
    /// Synchronizes authoritative HR-backed user fields.
    /// Returns true if any field changed.
    /// </summary>
    private bool SynchronizeUserFields(
        EmployeeUser user,
        EmployeeProfileInfo profile)
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

        if (!string.IsNullOrWhiteSpace(profile.MobileNumber) &&
            !string.Equals(
                user.PhoneNumber,
                profile.MobileNumber,
                StringComparison.Ordinal))
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

    private async Task<IResult<bool>> SynchronizeEmployeeProfileAsync(
        EmployeeUser user,
        EmployeeProfileInfo source,
        CancellationToken ct)
    {
        if (user is { EmployeeProfileId: not null, EmployeeProfile: null })
        {
            await dbContext.Entry(user)
                .Reference(x => x.EmployeeProfile)
                .LoadAsync(ct);
        }

        var target = user.EmployeeProfile;

        if (target is not null)
        {
            if (!HasProfileChanges(target, source))
            {
                return Result.Ok(false);
            }

            ApplyProfile(target, source);
            return Result.Ok(true);
        }

        var matchingProfiles = await dbContext.Set<EmployeeProfile>()
            .IgnoreQueryFilters()
            .Where(x => x.Qid == source.Qid)
            .Take(2)
            .ToListAsync(ct);

        if (matchingProfiles.Count > 1)
            return LogIdentityConflict(user.Id);

        target = matchingProfiles.SingleOrDefault();
        if (target is not null)
        {
            if (target.IsDeleted || !await CanLinkProfileAsync(user.Id, target.Id, ct))
                return LogIdentityConflict(user.Id, target.Id);

            user.EmployeeProfile = target;
            ApplyProfile(target, source);
            return Result.Ok(true);
        }

        target = new EmployeeProfile();
        user.EmployeeProfile = target;
        dbContext.Set<EmployeeProfile>().Add(target);
        ApplyProfile(target, source);

        return Result.Ok(true);
    }

    private async Task<bool> CanLinkProfileAsync(Guid userId, Guid profileId, CancellationToken ct)
    {
        var ownerIds = await dbContext.Set<EmployeeUser>()
            .IgnoreQueryFilters()
            .Where(x => x.EmployeeProfileId == profileId)
            .Select(x => x.Id)
            .Take(2)
            .ToListAsync(ct);

        return ownerIds.Count == 0 ||
               ownerIds.Count == 1 && ownerIds[0] == userId;
    }

    private async Task<IResult<EmployeeProfileInfo>> RecoverFromConcurrentProfileCreationAsync(
        EmployeeUser user,
        EmployeeProfileInfo source,
        CancellationToken ct)
    {
        var pendingProfile = user.EmployeeProfile;
        if (pendingProfile is not null &&
            dbContext.Entry(pendingProfile).State != EntityState.Detached)
        {
            dbContext.Entry(pendingProfile).State = EntityState.Detached;
        }

        user.EmployeeProfile = null;
        await dbContext.Entry(user).ReloadAsync(ct);

        var matchingProfiles = await dbContext.Set<EmployeeProfile>()
            .IgnoreQueryFilters()
            .Where(x => x.Qid == source.Qid)
            .Take(2)
            .ToListAsync(ct);

        if (matchingProfiles.Count != 1)
            return Result.Fail<EmployeeProfileInfo>(LogIdentityConflict(user.Id).Errors);

        var canonicalProfile = matchingProfiles[0];
        if (canonicalProfile.IsDeleted ||
            !await CanLinkProfileAsync(user.Id, canonicalProfile.Id, ct))
        {
            return Result.Fail<EmployeeProfileInfo>(
                LogIdentityConflict(user.Id, canonicalProfile.Id).Errors);
        }

        user.EmployeeProfile = canonicalProfile;
        ApplyProfile(canonicalProfile, source);
        SynchronizeUserFields(user, source);

        var retryResult = await userManager.UpdateAsync(user);
        if (!retryResult.Succeeded)
        {
            var errors = retryResult.Errors.Select(x => x.Description).ToArray();
            logger.Error(
                "Failed to persist HR employee profile after resolving a concurrent creation. UserId={UserId} Errors={Errors}",
                user.Id,
                string.Join(" | ", errors));
            return Result.Fail<EmployeeProfileInfo>(errors);
        }

        return Result.Ok(source);
    }

    private IResult<bool> LogIdentityConflict(Guid userId, Guid? profileId = null)
    {
        logger.Error(
            "Employee profile identity integrity conflict. UserId={UserId} EmployeeProfileId={EmployeeProfileId}",
            userId,
            profileId);
        return Result.Fail<bool>(ErrorsCodes.EmployeeProfileIdentityConflict);
    }

    private static bool IsEmployeeProfileUniqueConstraintViolation(DbUpdateException exception)
    {
        if (exception.InnerException is not SqlException sqlException)
            return false;

        return sqlException.Errors
            .Cast<SqlError>()
            .Any(error =>
                error.Number is 2601 or 2627 &&
                (error.Message.Contains("IX_EmployeeProfile_Qid", StringComparison.OrdinalIgnoreCase) ||
                 error.Message.Contains("IX_AspNetUsers_EmployeeProfileId", StringComparison.OrdinalIgnoreCase)));
    }

    private static void ApplyProfile(
        EmployeeProfile target,
        EmployeeProfileInfo source)
    {
        target.EmployeeNumber = source.EmployeeNumber;
        target.Qid = source.Qid;
        target.FullNameAr = source.FullNameAr;
        target.FullNameEn = source.FullNameEn;
        target.Email = source.Email;
        target.MobileNumber = source.MobileNumber;
        target.Department = source.Department;
        target.DepartmentNumber = source.DepartmentNumber;
        target.Section = source.Section;
        target.SectionNumber = source.SectionNumber;
        target.JobTitle = source.JobTitle;
        target.RawPayload = source.RawPayload;
    }
    
    private static bool HasProfileChanges(
        EmployeeProfile target,
        EmployeeProfileInfo source)
    {
        return
            !string.Equals(target.EmployeeNumber, source.EmployeeNumber, StringComparison.Ordinal) ||
            !string.Equals(target.Qid, source.Qid, StringComparison.Ordinal) ||
            !string.Equals(target.FullNameAr, source.FullNameAr, StringComparison.Ordinal) ||
            !string.Equals(target.FullNameEn, source.FullNameEn, StringComparison.Ordinal) ||
            !string.Equals(target.Email, source.Email, StringComparison.Ordinal) ||
            !string.Equals(target.MobileNumber, source.MobileNumber, StringComparison.Ordinal) ||
            !string.Equals(target.Department, source.Department, StringComparison.Ordinal) ||
            !string.Equals(target.DepartmentNumber, source.DepartmentNumber, StringComparison.Ordinal) ||
            !string.Equals(target.Section, source.Section, StringComparison.Ordinal) ||
            !string.Equals(target.SectionNumber, source.SectionNumber, StringComparison.Ordinal) ||
            !string.Equals(target.JobTitle, source.JobTitle, StringComparison.Ordinal) ||
            !string.Equals(target.RawPayload, source.RawPayload, StringComparison.Ordinal);
    }
}

