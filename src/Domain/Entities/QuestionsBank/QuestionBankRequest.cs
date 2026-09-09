using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankRequest), Schema = Schemas.Hr)]
public class QuestionBankRequest : EventEntity
{
    public Guid QuestionBankId { get; set; }

    public Guid? BaseVersionId { get; set; }

    public Guid RequestTypeId { get; set; }

    public Guid StatusId { get; set; }

    public int CurrentReviewRound { get; set; } = 0;

    public string? Reason { get; set; }

    public Guid SubmittedById { get; set; }

    public DateTime SubmittedAt { get; set; }

    public Guid? FinalDecisionById { get; set; }

    public DateTime? FinalDecisionAt { get; set; }

    public string? FinalDecisionNote { get; set; }

    public byte[]? RowVersion { get; set; }

    public QuestionBank QuestionBank { get; set; } = null!;

    public QuestionBankVersion? BaseVersion { get; set; }

    public QuestionBankRequestType RequestType { get; set; } = null!;

    public QuestionBankRequestStatus Status { get; set; } = null!;
    
    public EmployeeProfile SubmittedBy { get; set; } = null!;
    
    public EmployeeProfile? FinalDecisionBy { get; set; }

    public ICollection<QuestionBankRequestItem> Items { get; set; }
        = new List<QuestionBankRequestItem>();

    public ICollection<QuestionBankRequestReview> Reviews { get; set; }
        = new List<QuestionBankRequestReview>();

    public ICollection<QuestionBankRequestHistory> History { get; set; }
        = new List<QuestionBankRequestHistory>();
    
    public ICollection<QuestionBankAssignment> Assignments { get; set; }
        = new List<QuestionBankAssignment>();
}
