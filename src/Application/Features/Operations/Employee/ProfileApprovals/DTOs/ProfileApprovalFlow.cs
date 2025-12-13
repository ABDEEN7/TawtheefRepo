using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public static class ProfileApprovalFlow
{
    // نفس ترتيب واجهة المحقق (Stepper)
    public static readonly ProfileSection[] Sections =
    [
        ProfileSection.BasicInformation,
        ProfileSection.Contact,
        ProfileSection.Qualifications,
        ProfileSection.Experience,
        ProfileSection.TrainingCourses,
        ProfileSection.SkillsLanguages,
        ProfileSection.Attachments,
        ProfileSection.ProfilePhoto
    ];
}
