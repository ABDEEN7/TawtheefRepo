using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Lookups;

public static class QuestionChangeTypeIds
{
    public static readonly Guid ADD =
        Guid.Parse("dd984c21-7b25-444b-85a9-78767e337ddc");

    public static readonly Guid UPDATE =
        Guid.Parse("f7abde66-546b-4055-bfed-215c47bea845");

    public static readonly Guid DELETE =
        Guid.Parse("f02136e2-ac6c-4d3e-9c37-efd28587177d");
}

[Table(nameof(QuestionChangeType), Schema = Schemas.Lookup)]
public class QuestionChangeType : LookupBase
{
    public ICollection<QuestionBankRequestItem> RequestItems { get; set; } = new List<QuestionBankRequestItem>();
    public ICollection<QuestionBankVersionChange> VersionChanges { get; set; } = new List<QuestionBankVersionChange>();
}
