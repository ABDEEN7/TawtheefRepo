namespace Application.Operation.Features.Employee.Exams.DTOs;

public sealed record ExamJobSelectionDto(bool HasPendingApprovalExam, ExamConfigurationDto? ApprovedExam);
