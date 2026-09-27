using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamResultReportListItemDto
{
    public Guid Id { get; init; }

    public Guid ExamId { get; init; }
    public required string ExamNo { get; init; }
    public required string ExamTitle { get; init; }

    public Guid JobId { get; init; }
    public required string JobTitle { get; init; }

    public required DropdownOptions Status { get; init; }

    public int CandidateCount { get; init; }

    public int QualifiedForInterviewCount { get; init; }
    public int NotQualifiedForInterviewCount { get; init; }
    public int NoShowCount { get; init; }
    public int NotCompletedCount { get; init; }
    public int RequiresActionCount { get; init; }

    public DateTime? ApprovedAt { get; init; }

    public DateTime CreatedDate { get; init; }
    public DateTime LastUpdated { get; init; }
}
