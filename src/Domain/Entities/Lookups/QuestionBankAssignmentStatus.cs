using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class QuestionBankAssignmentStatusIds
{
    public static readonly Guid Assigned =
        Guid.Parse("9b38c5a0-fb16-44f2-8239-cb5ee704129d");

    public static readonly Guid QuestionEntryInProgress =
        Guid.Parse("8b73c382-c04b-49f7-bb78-4bc235df872f");

    public static readonly Guid QuestionEntryCompleted =
        Guid.Parse("c6cddace-de75-4a6e-8d1f-65e7e93ba4ed");

    public static readonly Guid ReturnedForModification =
        Guid.Parse("238e3c8d-84de-4d70-8a1f-192ac93224ba");

    public static readonly Guid ModificationCompleted =
        Guid.Parse("c035e63f-d866-488f-affd-9ae08a7043dd");

    public static readonly Guid Completed =
        Guid.Parse("a8ca906a-e1a3-44c9-87c1-8b4ec6192ab0");

    public static readonly Guid Cancelled =
        Guid.Parse("a4851953-f48a-4aa9-afb9-b51dfbc1e4d8");
}

[Table(nameof(QuestionBankAssignmentStatus), Schema = Schemas.Lookup)]
public class QuestionBankAssignmentStatus
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public string NameEn { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}
