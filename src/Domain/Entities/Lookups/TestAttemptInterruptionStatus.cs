using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestAttemptInterruptionStatusIds
{
    public static readonly Guid Open = Guid.Parse("b3aa445b-ebfe-4278-a79c-e59e105844aa");
    public static readonly Guid Resolved = Guid.Parse("b31c3a15-6d18-4e7b-bb38-c03e7baedc42");
}

[Table(nameof(TestAttemptInterruptionStatus), Schema = Schemas.Lookup)]
public class TestAttemptInterruptionStatus : LookupBase;
