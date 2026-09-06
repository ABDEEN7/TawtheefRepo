using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Lookups;

public static class QuestionBankRequestTypeIds
{
    public static readonly Guid CREATE =
        Guid.Parse("59659e5d-cf4c-46c5-bea1-3198a0f5a58c");

    public static readonly Guid MAINTENANCE =
        Guid.Parse("4fa8e626-6395-4987-b949-73cb070148d4");
}

[Table(nameof(QuestionBankRequestType), Schema = Schemas.Lookup)]
public class QuestionBankRequestType : LookupBase
{
    public ICollection<QuestionBankRequest> Requests { get; set; } = new List<QuestionBankRequest>();
}
