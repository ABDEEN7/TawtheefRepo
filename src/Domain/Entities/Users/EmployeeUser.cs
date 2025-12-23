using FluentResults;
using Tawtheef.Domain.Configurations.Rules;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class EmployeeUser : User
{
    public ICollection<ProfileAssignment> ProfileAssignments { get; set; } = [];
    
    public static Result<User> Register(string email,string displayName)
    {
        var name = FullName.TryParse(displayName);
        if (name.IsFailed) return Result.Fail<User>(name.Errors);

        var user = new EmployeeUser
        {
            Email = email,
            UserName = email,
            FullNameEn = name.Value.First + " " + name.Value.Last,
            FullNameAr = name.Value.First + " " + name.Value.Last,
            UserTypeId = UserTypeIds.Employee
        };

        return Result.Ok(user as User);
    }

    // domain behavior - attempt to create assignment and update profile
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
