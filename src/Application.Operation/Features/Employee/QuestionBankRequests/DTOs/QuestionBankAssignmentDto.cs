namespace Application.Operation.Features.Employee.QuestionBankRequests.DTOs;

public sealed record QuestionBankAssignmentDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string? EmployeeNameAr { get; init; }
    public string? EmployeeNameEn { get; init; }
    public Guid StatusId { get; init; }
    public required string StatusNameAr { get; init; }
    public required string StatusNameEn { get; init; }
    public int MinimumQuestionCount { get; init; }
    public string? Notes { get; init; }
    public DateTime AssignedAt { get; init; }
    public DateTime? QuestionEntryStartedAt { get; init; }
    public DateTime? QuestionEntryCompletedAt { get; init; }
    public DateTime? LastReturnedForModificationAt { get; init; }
    public DateTime? LastModificationCompletedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}
