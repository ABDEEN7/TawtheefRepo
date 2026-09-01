using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Profile.DTOs;

public sealed class MyProfileReviewSectionDto
{
    public ProfileSection Section { get; set; }

    // Reviewer notes count
    public int NotesCount { get; set; }
    public IReadOnlyList<MyProfileReviewNoteDto> Notes { get; set; } = [];
    public bool HasActionableSectionData { get; set; }

    // User changes inside this section (after review)
    public bool HasUserChanges { get; set; }
    public int PendingItemsCount { get; set; }
}
