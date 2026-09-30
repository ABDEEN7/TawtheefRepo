namespace Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;

public sealed record QuestionBankAssignmentWorkspaceDto(QuestionAssignmentInfoDto Assignment,
    QuestionAssignmentRequestDto Request, QuestionAssignmentBankDto QuestionBank,
    QuestionEntryProgressDto Progress, IReadOnlyCollection<AssignmentQuestionDto> Questions);
