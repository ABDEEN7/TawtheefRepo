using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public static class EmployeeSuperAdminIds
{
    public static readonly Guid EmployeeId1 = new("200c5018-fa8c-4ee7-a088-9077200b125c");
    public static readonly Guid EmployeeId2 = new("781561c3-0175-4165-80c1-7c6a79130b25");
    public static readonly Guid EmployeeId3 = new("0593ad82-e44e-4f55-aa08-c5c80764a873");
    public static readonly Guid EmployeeId4 = new("42e0d563-7603-453c-81b1-6b2325622b40");
}

[Index(nameof(EmployeeProfileId))]
public class EmployeeUser : User
{
    public Guid? EmployeeProfileId { get; init; }
    public EmployeeProfile? EmployeeProfile { get; set; }
    public ICollection<ProfileAssignment> ProfileAssignments { get; init; } = [];
    
    public static Result<User> Register(string email,string displayName)
    {
        var name = FullName.TryParse(displayName);
        if (name.IsFailed) return Result.Fail<User>(name.Errors);

        var userId = Guid.NewGuid();
        var user = new EmployeeUser
        {
            Id = userId,
            Email = email,
            UserName = email,
            FullNameEn = name.Value.First + " " + name.Value.Last,
            FullNameAr = name.Value.First + " " + name.Value.Last,
            UserTypeId = UserTypeIds.Employee
        };
        return Result.Ok<User>(user);
    }
    public Result<ProfileAssignment> CreateProfileAssignmentIfAllowed(UserProfile profile, int currentLoad, int assignedThisRound, int? perEmployeeLimit)
    {
        // Respect per-employee cap for this distribution run
        if (assignedThisRound >= perEmployeeLimit)
            return Result.Fail<ProfileAssignment>(ErrorsCodes.DistributionPerEmployeeLimitReached);

        // Ensure profile is assignable (caller may have already filtered, but guard here as domain rule)
        if (!ProfileDistributionRules.AssignableStatuses.Contains(profile.Status) &&
            profile.Status != UserProfileStatus.Approved)
            return Result.Fail<ProfileAssignment>(ErrorsCodes.ProfileNotAssignable);

        // Apply domain changes
        if (profile.Status != UserProfileStatus.Approved)
            profile.Status = UserProfileStatus.UnderReview;

        var assignment = ProfileAssignment.Assign(profile.Id, this.Id);

        // keep aggregate consistency in memory
        ProfileAssignments.Add(assignment);

        return Result.Ok(assignment);
    }
}
