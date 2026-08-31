using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class ExamStatusIds
{
    public static readonly Guid Draft = Guid.Parse("8d3322b5-9743-440d-a5d3-be71edc793a1");
    public static readonly Guid PendingApproval = Guid.Parse("3f33a442-953f-418c-af00-5220ee6a92b8");
    public static readonly Guid Returned = Guid.Parse("eca730c1-fdac-4b1b-a678-70a34cdc607f");
    public static readonly Guid Approved = Guid.Parse("a72cd921-edff-4f23-aea8-176b5b1ff3b7");
    public static readonly Guid Cancelled = Guid.Parse("e332af61-057e-4e65-a1a0-306be4ef41a1");
}

[Table(nameof(ExamStatus), Schema = Schemas.Lookup)]
public class ExamStatus : LookupBase;

