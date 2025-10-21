using CSharpFunctionalExtensions;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class EmployeeUser : User
{
    
    public static Result<User> Register(
        string email,
        string displayName,
        string genderRaw,
        string userTypeRaw
    )
    {
        var name = ValueObjects.User.FullName.TryParse(displayName);
        if (name.IsFailure) return name.ConvertFailure<User>();

        var gender = ValueObjects.User.Gender.TryFrom(genderRaw);
        if (gender.IsFailure) return gender.ConvertFailure<User>();

        var userType = UserTypeParser.TryFrom(userTypeRaw);
        if (userType.IsFailure) return userType.ConvertFailure<User>();

        var user = new EmployeeUser
        {
            Email = email,
            UserName = email,
            FirstName = name.Value.First,
            LastName = name.Value.Last,
            GenderId = gender.Value,
        };

        return Result.Success(user as User);
    }
}
