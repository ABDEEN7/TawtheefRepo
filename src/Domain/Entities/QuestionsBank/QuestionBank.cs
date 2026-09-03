using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.QuestionsBank;

[Table(nameof(QuestionBank), Schema = Schemas.Hr)]
public class QuestionBank : BaseEntity
{
    public Guid? ManagementId { get; set; }

    public Guid? JobTitleId { get; set; }
    
    public Guid QuestionBankTypeId { get; set; }

    public Guid? StageId { get; set; }

    public Guid? CurrentApprovedVersionId { get; set; }

    public bool IsActive { get; set; }

    public byte[]? RowVersion { get; set; }

    public Management? Management { get; set; } = null!;
    public JobTitle? JobTitle { get; set; } = null!;

    public Stage? Stage { get; set; } = null!;

    public QuestionBankVersion? CurrentApprovedVersion { get; set; }

    public QuestionBankType QuestionBankType { get; set; } = null!;
    
    public ICollection<QuestionBankVersion> Versions { get; set; }
        = new List<QuestionBankVersion>();

    public ICollection<QuestionBankRequest> Requests { get; set; }
        = new List<QuestionBankRequest>();

    public ICollection<QuestionBankVersionChange> VersionChanges { get; set; }
        = new List<QuestionBankVersionChange>();
}
