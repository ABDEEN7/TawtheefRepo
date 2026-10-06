using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests;

internal static class QuestionBankMaintenanceRules
{
    internal static readonly Guid[] OpenStatuses =
    [
        QuestionBankRequestStatusIds.PendingAssignment,
        QuestionBankRequestStatusIds.QuestionEntryInProgress,
        QuestionBankRequestStatusIds.PendingReview,
        QuestionBankRequestStatusIds.ModificationInProgress
    ];

    internal static string? GetMaintainabilityError(QuestionBank bank)
    {
        var version = bank.CurrentApprovedVersion;
        return !bank.IsActive || bank.CurrentApprovedVersionId is null || version is null ||
               version.IsDeleted || version.QuestionBankId != bank.Id || version.ApprovedAt is null
            ? ErrorsCodes.QuestionBankNotMaintainable
            : null;
    }
}
