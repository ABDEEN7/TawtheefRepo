using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankAssignment), Schema = Schemas.Hr)]
public class QuestionBankAssignment : EventEntity
{
    public Guid QuestionBankRequestId { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid AssignedByUserId { get; set; }

    public Guid StatusId { get; set; }

    public int MinimumQuestionCount { get; set; }

    public string? Notes { get; set; }

    public DateTime AssignedAt { get; set; }

    public DateTime? QuestionEntryStartedAt { get; set; }

    public DateTime? QuestionEntryCompletedAt { get; set; }

    public DateTime? LastReturnedForModificationAt { get; set; }

    public DateTime? LastModificationCompletedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public byte[]? RowVersion { get; set; }


    public QuestionBankRequest QuestionBankRequest { get; set; } = null!;

    public QuestionBankAssignmentStatus Status { get; set; } = null!;

    public EmployeeProfile Employee { get; set; } = null!;

    public EmployeeProfile AssignedByUser { get; set; } = null!;


    public ICollection<QuestionBankRequestItem> RequestItems { get; set; }
        = new List<QuestionBankRequestItem>();
}
