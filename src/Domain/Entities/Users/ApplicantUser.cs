using System.Net.Mail;
using CSharpFunctionalExtensions;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.ValueObjects.User;

namespace Tawtheef.Domain.Entities.Users;

public class ApplicantUser : User
{
    public Guid? ProfileId { get; set; }
    public UserProfile? Profile { get; set; }
    
    public ICollection<Qualification> Qualifications { get; set; } = [];
    public ICollection<Experience> Experiences { get; set; } = [];
    public ICollection<TrainingCourse> TrainingCourses { get; set; } = [];
    public ICollection<ApplicantSkill> Skills { get; set; } = [];
    public ICollection<LanguageProficiency> Languages { get; set; } = [];
    public ICollection<AdditionalAttachmentApplicant> AdditionalAttachments { get; set; } = [];
    
    public static Result<User> Register(string email,string displayName)
    {
        var name = ValueObjects.User.FullName.TryParse(displayName);
        if (name.IsFailure) return name.ConvertFailure<User>();

        var user = new ApplicantUser {
            Email = email,
            UserName = email,
            GivenNameEn = name.Value.First,
            FamilyNameEn = name.Value.Last,
            UserTypeId = UserTypeIds.Applicant
        };

        return Result.Success(user as User);
    }
}
