using Microsoft.EntityFrameworkCore;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

internal static class MaintenanceClaimConflict
{
    private const string UniqueIndexName = "IX_QuestionBankRequestItem_RequestId_QuestionId";

    internal static bool IsDuplicateQuestionClaim(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current.Message.Contains(UniqueIndexName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
