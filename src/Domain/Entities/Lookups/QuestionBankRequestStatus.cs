using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Lookups;

public static class QuestionBankRequestStatusIds
{
    public static readonly Guid PendingAssignment = Guid.Parse("4b25e8db-92f1-4b4c-baab-756a5d1fd3f8");
    public static readonly Guid QuestionEntryInProgress = Guid.Parse("66fbfd41-563e-4a12-a006-8a970285f62b");
    public static readonly Guid PendingReview = Guid.Parse("97c0610c-a992-43e1-b9a1-bfdbcf5b34b0");
    public static readonly Guid ModificationInProgress = Guid.Parse("f5cbde41-7e46-4793-953f-ebe5f8e31570");
    public static readonly Guid Issued = Guid.Parse("8b6d8c90-2e40-4957-ac52-bd46b0ae9726");
    public static readonly Guid Cancelled = Guid.Parse("77687c70-161f-4a6c-977e-95f86b37d724");
    public static readonly Guid Rejected = Guid.Parse("fa2e079c-9361-4e5a-9cbd-39dcb73a1520");
}

[Table(nameof(QuestionBankRequestStatus), Schema = Schemas.Lookup)]
public class QuestionBankRequestStatus : LookupBase
{
    public ICollection<QuestionBankRequest> Requests { get; set; } = new List<QuestionBankRequest>();
    public ICollection<QuestionBankRequestHistory> FromHistories { get; set; } = new List<QuestionBankRequestHistory>();
    public ICollection<QuestionBankRequestHistory> ToHistories { get; set; } = new List<QuestionBankRequestHistory>();
}
