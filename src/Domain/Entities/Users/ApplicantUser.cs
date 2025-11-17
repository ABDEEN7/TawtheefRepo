using CSharpFunctionalExtensions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class ApplicantUser : User
{
    public UserProfile? Profile { get; set; }
    public ICollection<Invitation> Invitations { get; init; } = [];
    public static Result<User> Register(string email,string displayName)
    {
        var name = FullName.TryParse(displayName);
        if (name.IsFailure) return name.ConvertFailure<User>();

        var user = new ApplicantUser {
            Email = email,
            UserName = email,
            FullNameEn = name.Value.First + " " + name.Value.Last,
            FullNameAr = name.Value.First + " " + name.Value.Last,
            UserTypeId = UserTypeIds.Applicant
        };

        return Result.Success(user as User);
    }
}
