using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(Question), Schema = Schemas.Hr)]
public class Question : EventEntity
{
    public string? QuestionCode { get; set; }
    
    public ICollection<QuestionRevision> Revisions { get; set; }
        = new List<QuestionRevision>();

    public ICollection<QuestionBankVersionQuestion> BankVersionQuestions { get; set; }
        = new List<QuestionBankVersionQuestion>();

    public ICollection<QuestionBankRequestItem> RequestItems { get; set; }
        = new List<QuestionBankRequestItem>();

    public ICollection<QuestionBankVersionChange> VersionChanges { get; set; }
        = new List<QuestionBankVersionChange>();
}
