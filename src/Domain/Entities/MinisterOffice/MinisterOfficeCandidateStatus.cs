namespace Tawtheef.Domain.Entities.MinisterOffice;

/// <summary>
/// Computed status for Minister Office candidates — NOT stored on the entity.
/// Derived at query time from UserProfile.Status + Invitation records.
/// </summary>
public enum MinisterOfficeCandidateStatus
{
    NoProfile = 0,
    DraftProfile = 1,
    SubmittedForApproval = 2,
    ReturnedForCorrection = 3,
    ApprovedProfile = 4,
    InvitationsReceived = 5
}
