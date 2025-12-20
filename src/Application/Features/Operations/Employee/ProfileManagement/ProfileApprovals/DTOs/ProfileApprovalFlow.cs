using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

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
