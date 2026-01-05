using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class MyProfileReviewSummaryDto
{
    public Guid UserProfileId { get; set; }
    public UserProfileStatus ProfileStatus { get; set; }

    // ===== Reviewer Notes =====
    public IReadOnlyList<MyProfileReviewSectionDto> Sections { get; set; } = [];
    public int TotalNotes { get; set; }

    // ===== User Saved Changes (Phase 2) =====
    public bool HasSavedChanges { get; set; }
    public IReadOnlyList<ProfileSection> ChangedSections { get; set; } = [];

    // Optional but very useful for UX:
    public DateTimeOffset? LastReviewerActionAtUtc { get; set; }
    public DateTimeOffset? LastUserChangeAtUtc { get; set; }

    // Show CTA "Resubmit"
    public bool CanResubmit { get; set; }
}
