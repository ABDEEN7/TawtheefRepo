using CSharpFunctionalExtensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class EmployeeUser : User
{
    
    public static Result<User> Register(string email,string displayName)
    {
        var name = ValueObjects.User.FullName.TryParse(displayName);
        if (name.IsFailure) return name.ConvertFailure<User>();

        var user = new EmployeeUser
        {
            Email = email,
            UserName = email,
            GivenNameEn = name.Value.First,
            FamilyNameEn = name.Value.Last,
            UserTypeId = UserTypeIds.Employee
        };

        return Result.Success(user as User);
    }
}
