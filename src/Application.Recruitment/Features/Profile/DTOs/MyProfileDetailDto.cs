using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.DTOs;

public sealed class MyProfileDetailDto
{
    public Guid UserId { get; init; }
    public Guid UserProfileId { get; init; }

    public UserProfileStatus ProfileStatus { get; init; }

    // What the user sees and edits
    public ProfileApprovalDataDto DraftProfile { get; init; } = default!;

    // Optional for compare UX (if you can resolve it now)
    public ProfileApprovalDataDto? ApprovedProfile { get; init; }

    public ProfileEditPermissionsDto Permissions { get; init; } = default!;
}

public sealed class ProfileEditPermissionsDto
{
    public bool CanEditPrerequisites { get; init; }
    public bool CanEditPersonal { get; init; }
    public bool CanEditContact { get; init; }

    public bool CanAddQualifications { get; init; }
    public bool CanAddExperiences { get; init; }
    public bool CanAddTrainingCourses { get; init; }
    public bool CanAddCertificatesAndAwards { get; init; }

    public bool CanEditSkills { get; init; }
    public bool CanEditLanguages { get; init; }
    public bool CanAddAttachments { get; init; }

    public bool CanSubmit { get; init; }
    public bool IsLockedBecauseUnderReview { get; init; }
}
