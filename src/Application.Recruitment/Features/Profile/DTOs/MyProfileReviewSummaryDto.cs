using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Recruitment.Features.Profile.DTOs;

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
    public IReadOnlyList<MyProfileReviewChangedItemDto> ChangedItems { get; set; } = [];

    // Optional but very useful for UX:
    public DateTimeOffset? LastReviewerActionAtUtc { get; set; }
    public DateTimeOffset? LastUserChangeAtUtc { get; set; }

    // Show CTA "Resubmit"
    public bool CanResubmit { get; set; }
}

public sealed class MyProfileReviewChangedItemDto
{
    public Guid ReviewItemId { get; set; }
    public ProfileSection Section { get; set; }
    public ReviewTargetType TargetType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? FieldPath { get; set; }
    public Guid? EntityId { get; set; }
    public string? EntityName { get; set; }
    public Guid? ResourceId { get; set; }
}
