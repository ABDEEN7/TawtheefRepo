using System.Text.Json;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Logger;

namespace Application.Operation.Features.Employee.TestSessions;

internal sealed record TestSessionWorkflowMetadata(
    Guid ReviewerId,
    DateTime DecisionAt,
    string PreviousStatus,
    string NewStatus)
{
    internal void ApplyDecision(TestSession testSession, Guid statusId) => testSession.StatusId = statusId;

    internal ActionLog CreateActionLog(TestSession testSession, string actionType, string? returnNote = null) => new()
    {
        UserId = ReviewerId,
        LogType = ActionLogType.Employee,
        ActionType = actionType,
        Section = "TestSessionWorkflow",
        EntityId = testSession.Id,
        Notes = JsonSerializer.Serialize(new
        {
            testSessionId = testSession.Id,
            sessionNo = testSession.SessionNo,
            previousStatus = PreviousStatus,
            status = NewStatus,
            performedBy = ReviewerId,
            performedAt = DecisionAt,
            returnNote,
        }),
    };
}
