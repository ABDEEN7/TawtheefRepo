using System.Linq.Expressions;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

internal static class QuestionAssignmentProgress
{
    // DELETE remains part of the workflow, but has no usable proposed question to count.
    public static readonly Expression<Func<QuestionBankRequestItem, bool>> Countable = item =>
        !item.IsDeleted &&
        item.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST &&
        item.StatusId != QuestionBankRequestItemStatusIds.REJECTED &&
        item.ChangeTypeId != QuestionChangeTypeIds.DELETE;

    private static readonly Func<QuestionBankRequestItem, bool> IsCountable = Countable.Compile();

    public static int Count(IEnumerable<QuestionBankRequestItem> items, Guid assignmentId, Guid requestId) =>
        items.Count(item => item.QuestionBankAssignmentId == assignmentId &&
                            item.RequestId == requestId && IsCountable(item));
}
