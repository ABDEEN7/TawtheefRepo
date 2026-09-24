using Tawtheef.Application.Common.Models;

namespace Application.Operation.Features.Employee.TestSessions.DTOs;

public sealed record TestSessionListItemDto
{
    public Guid Id { get; init; }
    public int SessionNo { get; init; }
    public Guid ExamId { get; init; }
    public required string ExamName { get; init; }
    public Guid JobId { get; init; }
    public required string JobTitle { get; init; }
    public DateOnly SessionDate { get; init; }
    public Guid PeriodId { get; init; }
    public required string Period { get; init; }
    public Guid RoomId { get; init; }
    public required string RoomName { get; init; }
    public int CandidateCount { get; init; }
    public string? RoomHeadName { get; init; }
    public required DropdownOptions Status { get; init; }
}
