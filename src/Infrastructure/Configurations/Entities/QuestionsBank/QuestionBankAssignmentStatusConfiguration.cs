using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Infrastructure.Configurations.Entities.Lookups;

public class QuestionBankAssignmentStatusConfiguration
    : IEntityTypeConfiguration<QuestionBankAssignmentStatus>
{
    public void Configure(EntityTypeBuilder<QuestionBankAssignmentStatus> builder)
    {
        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.NameAr)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.NameEn)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.HasData(
            new QuestionBankAssignmentStatus
            {
                Id = QuestionBankAssignmentStatusIds.Assigned,
                Code = "ASSIGNED",
                NameEn = "Assigned",
                NameAr = "تم الإسناد",
                DisplayOrder = 1,
                IsActive = true
            },
            new QuestionBankAssignmentStatus
            {
                Id = QuestionBankAssignmentStatusIds.QuestionEntryInProgress,
                Code = "QUESTION_ENTRY_IN_PROGRESS",
                NameEn = "Question Entry In Progress",
                NameAr = "قيد إدخال الأسئلة",
                DisplayOrder = 2,
                IsActive = true
            },
            new QuestionBankAssignmentStatus
            {
                Id = QuestionBankAssignmentStatusIds.QuestionEntryCompleted,
                Code = "QUESTION_ENTRY_COMPLETED",
                NameEn = "Question Entry Completed",
                NameAr = "تم إنهاء إدخال الأسئلة",
                DisplayOrder = 3,
                IsActive = true
            },
            new QuestionBankAssignmentStatus
            {
                Id = QuestionBankAssignmentStatusIds.ReturnedForModification,
                Code = "RETURNED_FOR_MODIFICATION",
                NameEn = "Returned for Modification",
                NameAr = "معاد للتعديل",
                DisplayOrder = 4,
                IsActive = true
            },
            new QuestionBankAssignmentStatus
            {
                Id = QuestionBankAssignmentStatusIds.ModificationCompleted,
                Code = "MODIFICATION_COMPLETED",
                NameEn = "Modification Completed",
                NameAr = "تم إنهاء التعديلات",
                DisplayOrder = 5,
                IsActive = true
            },
            new QuestionBankAssignmentStatus
            {
                Id = QuestionBankAssignmentStatusIds.Completed,
                Code = "COMPLETED",
                NameEn = "Completed",
                NameAr = "مكتمل",
                DisplayOrder = 6,
                IsActive = true
            },
            new QuestionBankAssignmentStatus
            {
                Id = QuestionBankAssignmentStatusIds.Cancelled,
                Code = "CANCELLED",
                NameEn = "Cancelled",
                NameAr = "ملغي",
                DisplayOrder = 7,
                IsActive = true
            }
        );
    }
}
