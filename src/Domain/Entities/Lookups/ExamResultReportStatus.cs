using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class ExamResultReportStatusIds
{
    public static readonly Guid Creating = Guid.Parse("ddff4a54-538e-4082-a799-97f6243f3d26");
    public static readonly Guid UnderReview = Guid.Parse("3369b08f-2dd3-4485-a899-1515c63ce8b1");
    public static readonly Guid Returned = Guid.Parse("e5032918-cb5d-4ca4-a10a-95002a1e97a0");
    public static readonly Guid Approved = Guid.Parse("bac8cd24-bbb0-40ac-a800-9fe838622ad8");
    public static readonly Guid Closed = Guid.Parse("d2afd367-0383-44cf-acd6-dcf563acb053");
}

[Table(nameof(ExamResultReportStatus), Schema = Schemas.Lookup)]
public class ExamResultReportStatus : LookupBase;

