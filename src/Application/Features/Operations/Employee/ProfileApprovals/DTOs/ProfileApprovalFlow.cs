using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public static class ProfileApprovalFlow
{
    // نفس ترتيب واجهة المحقق (Stepper)
    public static readonly ProfileSection[] Sections =
    [
        ProfileSection.Personal,
        ProfileSection.Contact,
        ProfileSection.Qualifications,
        ProfileSection.Experience,
        ProfileSection.TrainingCourses,
        ProfileSection.CertificatesAndAwards,
        ProfileSection.Skills,
        ProfileSection.Languages,
        ProfileSection.Attachments
    ];
}
