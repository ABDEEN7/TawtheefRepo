using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.ProfileApproval;

public static class ProfileApprovalFlow
{
    public static readonly ProfileSection[] Sections =
    [
        ProfileSection.Prerequisites,
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
