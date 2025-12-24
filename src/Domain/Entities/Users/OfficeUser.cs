using Tawtheef.Domain.Entities.Lookups.NoneSeeds;
using FluentResults;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class OfficeUser : User
{
    public Guid OfficeId { get; set; }
    public Office? Office { get; set; }
    
    public static Result<User> Register(string email, Guid officeId, string fullNameAr, string fullNameEn)
    {
        var nameAr = FullName.TryParse(fullNameAr);
        if (nameAr.IsFailed) return Result.Fail<User>(nameAr.Errors);
        var nameEn = FullName.TryParse(fullNameEn);
        if (nameEn.IsFailed) return Result.Fail<User>(nameEn.Errors);
        var user = new OfficeUser() {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            OfficeId = officeId,
            FullNameEn = nameAr.Value.First + " " + nameAr.Value.Last,
            FullNameAr = nameEn.Value.First + " " + nameEn.Value.Last,
            UserTypeId = UserTypeIds.OfficeUser
        };

        return Result.Ok(user as User);
    }
}
