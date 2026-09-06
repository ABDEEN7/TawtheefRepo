using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Tawtheef.Domain.Entities.Lookups;

public static class QuestionBankRequestItemStatusIds
{
    public static readonly Guid DRAFT =
        Guid.Parse("19760735-e5f0-4236-9fff-8487f9f057ef");

    public static readonly Guid PENDING_REVIEW =
        Guid.Parse("c5ed702c-cfca-4d7e-8bf9-98cdf15f2106");

    public static readonly Guid APPROVED =
        Guid.Parse("1914dd04-27d9-468d-b03e-813df6ce87ff");

    public static readonly Guid NEEDS_MODIFICATION =
        Guid.Parse("bfa1d9c1-a284-452b-8b59-a2d5fc0a6dfe");

    public static readonly Guid REJECTED =
        Guid.Parse("217164f8-c23d-4913-a549-16f80186f9cb");

    public static readonly Guid REMOVED_FROM_REQUEST =
        Guid.Parse("f48eedf5-4fe9-4ded-931b-c76bd5c9ba80");
}

[Table(nameof(QuestionBankRequestItemStatus), Schema = Schemas.Lookup)]
public class QuestionBankRequestItemStatus : LookupBase
{
    public ICollection<QuestionBankRequestItem> RequestItems { get; set; } = new List<QuestionBankRequestItem>();
}
