using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class MyProfileReviewNoteDto
{
    public Guid ReviewItemId { get; set; }

    public ReviewTargetType TargetType { get; set; }
    public ReviewStatus Status { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Note { get; set; }

    public string? FieldPath { get; set; }
    public Guid? EntityId { get; set; }
    public string? EntityName { get; set; }

    public Guid? ResourceId { get; set; }
    public DateTimeOffset? ReviewedAtUtc { get; set; }
}
