using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestAttemptInterruptionResolutionActionIds
{
    public static readonly Guid Resume = Guid.Parse("76698ef2-ed94-47e0-8772-72a49372f384");
    public static readonly Guid Reschedule = Guid.Parse("4ec227a3-e99b-4f7f-b7e7-c732a3d58b29");
    public static readonly Guid Cancel = Guid.Parse("a10637d3-dd47-4cf0-8c0f-9e480d7e0576");
}

[Table(nameof(TestAttemptInterruptionResolutionAction), Schema = Schemas.Lookup)]
public class TestAttemptInterruptionResolutionAction : LookupBase;
