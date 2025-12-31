using FluentResults;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Users;

public class OfficeUser : User
{
    public Guid? OfficeId { get; set; }
    public Office? Office { get; init; }
    
    public static Result<User> Register(string email, string fullName)
    {
        var user = new OfficeUser() {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            FullNameEn = fullName,
            FullNameAr = fullName,
            UserTypeId = UserTypeIds.OfficeUser
        };

        return Result.Ok<User>(user);
    }
}
