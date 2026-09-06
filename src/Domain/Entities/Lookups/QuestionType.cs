using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Lookups;

public static class QuestionTypeIds
{
    public static Guid MULTIPLE_CHOICE = Guid.Parse("04a6e083-d38f-4838-a820-402fa4c53a39");
    public static Guid TRUE_FALSE = Guid.Parse("62d2e3f9-562d-45f1-ace4-867b929cae43");
}

[Table(nameof(QuestionType), Schema = Schemas.Lookup)]
public class QuestionType : LookupBase
{
    public ICollection<QuestionRevision> QuestionRevisions { get; set; } = new List<QuestionRevision>();
}
