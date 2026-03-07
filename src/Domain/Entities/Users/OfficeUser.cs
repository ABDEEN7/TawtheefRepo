using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Users;

[Index(nameof(OfficeId))]
public class OfficeUser : User
{
    public Guid? OfficeId { get; set; }
    public Office? Office { get; init; }
    
    public static Result<User> Register(string email, string fullName)
    {
        return Register(email, fullName, fullName);
    }

    public static Result<User> Register(string email, string fullNameAr, string fullNameEn)
    {
        var user = new OfficeUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            FullNameEn = fullNameEn,
            FullNameAr = fullNameAr,
            UserTypeId = UserTypeIds.OfficeUser
        };

        return Result.Ok<User>(user);
    }
}
