using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamListItemDto
{
    public Guid Id { get; init; }
    public required string ExamNo { get; init; }
    public required string JobTitle { get; init; }
    public required List<string> Specializations { get; init; }
    public int TotalQuestions { get; init; }
    public int TotalDurationMinutes { get; init; }
    public required DropdownOptions Status { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime LastUpdated { get; init; }
}
