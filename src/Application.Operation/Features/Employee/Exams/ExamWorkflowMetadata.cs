using System.Text.Json;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;

namespace Application.Operation.Features.Employee.Exams;

internal sealed record ExamWorkflowMetadata(
    Guid ReviewerId,
    DateTime DecisionAt,
    string PreviousStatus,
    string NewStatus)
{
    internal void ApplyDecision(Exam exam, Guid statusId)
    {
        exam.StatusId = statusId;
        exam.DecisionById = ReviewerId;
        exam.DecisionAt = DecisionAt;
    }

    internal ActionLog CreateActionLog(Exam exam, string actionType, string? returnNote = null) => new()
    {
        UserId = ReviewerId,
        LogType = ActionLogType.Employee,
        ActionType = actionType,
        Section = "ExamWorkflow",
        EntityId = exam.Id,
        Notes = JsonSerializer.Serialize(new
        {
            examId = exam.Id,
            examNumber = exam.ExamNo,
            previousStatus = PreviousStatus,
            status = NewStatus,
            performedBy = ReviewerId,
            performedAt = DecisionAt,
            returnNote,
        }),
    };
}
