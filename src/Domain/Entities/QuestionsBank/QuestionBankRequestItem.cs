using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankRequestItem), Schema = Schemas.Hr)]
public class QuestionBankRequestItem : BaseEntity
{
    public Guid RequestId { get; set; }

    public Guid QuestionBankAssignmentId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid ChangeTypeId { get; set; }

    public Guid StatusId { get; set; }

    public Guid? OriginalRevisionId { get; set; }

    public Guid? CurrentProposedRevisionId { get; set; }

    public Guid? RemovedById { get; set; }

    public DateTime? RemovedAt { get; set; }

    public string? RemovalNote { get; set; }

    public byte[]? RowVersion { get; set; }


    public QuestionBankRequest Request { get; set; } = null!;

    public QuestionBankAssignment QuestionBankAssignment { get; set; } = null!;

    public Question Question { get; set; } = null!;

    public QuestionChangeType ChangeType { get; set; } = null!;

    public QuestionBankRequestItemStatus Status { get; set; } = null!;

    public QuestionRevision? OriginalRevision { get; set; }

    public QuestionRevision? CurrentProposedRevision { get; set; }
    
    public EmployeeProfile? RemovedBy { get; set; }
    
    public ICollection<QuestionBankRequestItemReview> Reviews { get; set; }
        = new List<QuestionBankRequestItemReview>();
}
