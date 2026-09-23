using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;

public sealed record OperationalIssueDto(
    Guid Id,
    Guid InterviewAppointmentId,
    OperationalIssueType IssueType,
    string? Description,
    bool IsBlocking,
    OperationalIssueStatus Status,
    Guid? ResolvedById,
    DateTimeOffset? ResolvedAt,
    string? ResolutionNotes,
    DateTimeOffset CreatedDate);
