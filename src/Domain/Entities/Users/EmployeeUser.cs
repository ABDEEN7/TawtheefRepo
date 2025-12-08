using FluentResults;
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
}
