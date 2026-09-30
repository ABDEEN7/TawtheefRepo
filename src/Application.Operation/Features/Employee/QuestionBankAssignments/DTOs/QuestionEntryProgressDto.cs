namespace Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;

public sealed record QuestionEntryProgressDto(int MinimumQuestionCount, int CurrentQuestionCount,
    int RemainingQuestionCount, bool CanFinish);
