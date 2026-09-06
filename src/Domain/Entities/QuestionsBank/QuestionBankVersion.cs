using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBankVersion), Schema = Schemas.Hr)]
public class QuestionBankVersion : BaseEntity
{

    public Guid QuestionBankId { get; set; }

    public int VersionNo { get; set; }

    public Guid? PreviousVersionId { get; set; }

    public Guid CreatedFromRequestId { get; set; }

    public Guid? ApprovedById { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
    
    public QuestionBank QuestionBank { get; set; } = null!;

    public QuestionBankVersion? PreviousVersion { get; set; }

    public ICollection<QuestionBankVersion> NextVersions { get; set; }
        = new List<QuestionBankVersion>();

    public QuestionBankRequest CreatedFromRequest { get; set; } = null!;

    public ICollection<QuestionBankVersionQuestion> Questions { get; set; }
        = new List<QuestionBankVersionQuestion>();
}
