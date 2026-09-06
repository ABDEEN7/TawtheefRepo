using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;

namespace Seeds.Importing.Lookups;

internal static class QuestionBankLookupImporter
{
    public static async Task ImportAsync(
        SeedImportContext context,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(context);

        await UpsertLookupAsync<QuestionBankType>(
            context,
            [
                new(QuestionBankTypeIds.SPECIALIZED, "SPECIALIZED", "تخصصي", "Specialized", 1),
                new(QuestionBankTypeIds.SKILLS, "SKILLS", "مهارات", "Skills", 2),
                new(QuestionBankTypeIds.EDUCATIONAL, "EDUCATIONAL", "تربوي", "Educational", 3)
            ],
            ct);

        await UpsertLookupAsync<QuestionType>(
            context,
            [
                new(QuestionTypeIds.MULTIPLE_CHOICE, "MULTIPLE_CHOICE", "اختيار من متعدد", "Multiple Choice", 1),
                new(QuestionTypeIds.TRUE_FALSE, "TRUE_FALSE", "صح / خطأ", "True / False", 2)
            ],
            ct);

        await UpsertLookupAsync<DifficultyLevel>(
            context,
            [
                new(DifficultyLevelIds.EASY, "EASY", "سهل", "Easy", 1),
                new(DifficultyLevelIds.MEDIUM, "MEDIUM", "متوسط", "Medium", 2),
                new(DifficultyLevelIds.HARD, "HARD", "صعب", "Hard", 3)
            ],
            ct);

        await UpsertLookupAsync<Stage>(
            context,
            [
                new(StageIds.Primary, "PRIMARY", "المرحلة الابتدائية", "Primary", 1),
                new(StageIds.Preparatory, "PREPARATORY", "المرحلة الإعدادية", "Preparatory", 2),
                new(StageIds.Secondary, "SECONDARY", "المرحلة الثانوية", "Secondary", 3)
            ],
            ct);

        await UpsertLookupAsync<QuestionBankRequestType>(
            context,
            [
                new(QuestionBankRequestTypeIds.CREATE, "CREATE", "إنشاء", "Create", 1),
                new(QuestionBankRequestTypeIds.MAINTENANCE, "MODIFY", "تعديل", "Modify", 2)
            ],
            ct);

        await UpsertLookupAsync<QuestionBankRequestStatus>(
            context,
            [
                new(QuestionBankRequestStatusIds.PendingAssignment, "PENDING_ASSIGNMENT", "بانتظار الإسناد", "Pending Assignment", 1),
                new(QuestionBankRequestStatusIds.QuestionEntryInProgress, "QUESTION_ENTRY_IN_PROGRESS", "قيد إدخال الأسئلة", "Question Entry In Progress", 2),
                new(QuestionBankRequestStatusIds.PendingReview, "PENDING_REVIEW", "بانتظار المراجعة", "Pending Review", 3),
                new(QuestionBankRequestStatusIds.ModificationInProgress, "MODIFICATION_IN_PROGRESS", "قيد التعديل", "Modification In Progress", 4),
                new(QuestionBankRequestStatusIds.Issued, "ISSUED", "تم الإصدار", "Issued", 5),
                new(QuestionBankRequestStatusIds.Cancelled, "CANCELLED", "ملغي", "Cancelled", 6),
                new(QuestionBankRequestStatusIds.Rejected, "REJECTED", "مرفوض", "Rejected", 7)
            ],
            ct);

        await UpsertAssignmentStatusesAsync(
            context,
            [
                new(QuestionBankAssignmentStatusIds.Assigned, "ASSIGNED", "تم الإسناد", "Assigned", 1),
                new(QuestionBankAssignmentStatusIds.QuestionEntryInProgress, "QUESTION_ENTRY_IN_PROGRESS", "قيد إدخال الأسئلة", "Question Entry In Progress", 2),
                new(QuestionBankAssignmentStatusIds.QuestionEntryCompleted, "QUESTION_ENTRY_COMPLETED", "تم إنهاء إدخال الأسئلة", "Question Entry Completed", 3),
                new(QuestionBankAssignmentStatusIds.ReturnedForModification, "RETURNED_FOR_MODIFICATION", "معاد للتعديل", "Returned for Modification", 4),
                new(QuestionBankAssignmentStatusIds.ModificationCompleted, "MODIFICATION_COMPLETED", "تم إنهاء التعديلات", "Modification Completed", 5),
                new(QuestionBankAssignmentStatusIds.Completed, "COMPLETED", "مكتمل", "Completed", 6),
                new(QuestionBankAssignmentStatusIds.Cancelled, "CANCELLED", "ملغي", "Cancelled", 7)
            ],
            ct);

        await UpsertLookupAsync<QuestionBankRequestItemStatus>(
            context,
            [
                new(QuestionBankRequestItemStatusIds.DRAFT, "DRAFT", "مسودة", "Draft", 1),
                new(QuestionBankRequestItemStatusIds.PENDING_REVIEW, "PENDING_REVIEW", "بانتظار المراجعة", "Pending Review", 2),
                new(QuestionBankRequestItemStatusIds.APPROVED, "APPROVED", "معتمد", "Approved", 3),
                new(QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION, "NEEDS_MODIFICATION", "يحتاج إلى تعديل", "Needs Modification", 4),
                new(QuestionBankRequestItemStatusIds.REJECTED, "REJECTED", "مرفوض", "Rejected", 5),
                new(QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST, "REMOVED_FROM_REQUEST", "تمت إزالته من الطلب", "Removed from Request", 6)
            ],
            ct);

        await UpsertLookupAsync<QuestionChangeType>(
            context,
            [
                new(QuestionChangeTypeIds.ADD, "ADD", "إضافة", "Add", 1),
                new(QuestionChangeTypeIds.UPDATE, "UPDATE", "تعديل", "Update", 2),
                new(QuestionChangeTypeIds.DELETE, "DELETE", "حذف", "Delete", 3)
            ],
            ct);

        await UpsertLookupAsync<QuestionReviewDecision>(
            context,
            [
                new(QuestionReviewDecisionIds.APPROVED, "APPROVED", "معتمد", "Approved", 1),
                new(QuestionReviewDecisionIds.NEEDS_MODIFICATION, "NEEDS_MODIFICATION", "يحتاج إلى تعديل", "Needs Modification", 2),
                new(QuestionReviewDecisionIds.REJECTED, "REJECTED", "مرفوض", "Rejected", 3)
            ],
            ct);
    }

    private static async Task UpsertLookupAsync<TLookup>(
        SeedImportContext context,
        IReadOnlyCollection<LookupSeed> seeds,
        CancellationToken ct)
        where TLookup : LookupBase
    {
        var set = context.Db.Set<TLookup>();

        var existing = await set
            .IgnoreQueryFilters()
            .ToListAsync(ct);

        foreach (var seed in seeds)
        {
            ct.ThrowIfCancellationRequested();

            var entity = existing.FirstOrDefault(x => x.Id == seed.Id);

            var backendConflict = existing.FirstOrDefault(x =>
                x.Id != seed.Id &&
                string.Equals(
                    x.BackendName,
                    seed.BackendName,
                    StringComparison.OrdinalIgnoreCase));

            if (backendConflict is not null)
            {
                throw new InvalidOperationException(
                    $"Cannot seed {typeof(TLookup).Name} '{seed.BackendName}'. " +
                    $"BackendName is already used by Id {backendConflict.Id}.");
            }

            if (entity is null)
            {
                entity = Activator.CreateInstance<TLookup>()
                         ?? throw new InvalidOperationException(
                             $"Could not create instance of {typeof(TLookup).Name}.");

                entity.Id = seed.Id;
                entity.BackendName = seed.BackendName;
                entity.NameAr = seed.NameAr;
                entity.NameEn = seed.NameEn;
                entity.DisplayOrder = seed.DisplayOrder;
                entity.IsDeleted = false;

                set.Add(entity);
                existing.Add(entity);

                continue;
            }

            entity.BackendName = seed.BackendName;
            entity.NameAr = seed.NameAr;
            entity.NameEn = seed.NameEn;
            entity.DisplayOrder = seed.DisplayOrder;
            entity.IsDeleted = false;
        }
    }

    private static async Task UpsertAssignmentStatusesAsync(
        SeedImportContext context,
        IReadOnlyCollection<AssignmentStatusSeed> seeds,
        CancellationToken ct)
    {
        var set = context.Db.Set<QuestionBankAssignmentStatus>();
        var existing = await set.ToListAsync(ct);

        foreach (var seed in seeds)
        {
            ct.ThrowIfCancellationRequested();

            var entity = existing.FirstOrDefault(x => x.Id == seed.Id);
            var codeConflict = existing.FirstOrDefault(x =>
                x.Id != seed.Id &&
                string.Equals(
                    x.Code,
                    seed.Code,
                    StringComparison.OrdinalIgnoreCase));

            if (codeConflict is not null)
            {
                throw new InvalidOperationException(
                    $"Cannot seed {nameof(QuestionBankAssignmentStatus)} '{seed.Code}'. " +
                    $"Code is already used by Id {codeConflict.Id}.");
            }

            if (entity is null)
            {
                entity = new QuestionBankAssignmentStatus
                {
                    Id = seed.Id
                };

                set.Add(entity);
                existing.Add(entity);
            }

            entity.Code = seed.Code;
            entity.NameAr = seed.NameAr;
            entity.NameEn = seed.NameEn;
            entity.DisplayOrder = seed.DisplayOrder;
            entity.IsActive = true;
        }
    }

    private sealed record LookupSeed(
        Guid Id,
        string BackendName,
        string NameAr,
        string NameEn,
        int DisplayOrder);

    private sealed record AssignmentStatusSeed(
        Guid Id,
        string Code,
        string NameAr,
        string NameEn,
        int DisplayOrder);
}
