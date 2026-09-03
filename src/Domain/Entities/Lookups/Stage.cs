using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Lookups;

public static class StageIds
{
    public static Guid Primary = Guid.Parse("cc8ed439-fc4b-442c-b998-a190d0a4e97e");
    public static Guid Preparatory = Guid.Parse("34265c85-30ca-40ac-abaa-560d175860e0");
    public static Guid Secondary = Guid.Parse("362bf79c-b5da-4145-86ea-b4babdd7e022");
}

[Table(nameof(Stage), Schema = Schemas.Lookup)]
public class Stage : LookupBase
{
    public ICollection<QuestionBank> QuestionBanks { get; set; }
        = new List<QuestionBank>();
}
