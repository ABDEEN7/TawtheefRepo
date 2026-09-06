using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestAttemptStatusIds
{
    public static readonly Guid NotStarted = Guid.Parse("f31ffad4-c04a-451a-abdd-877e4e9919a6");
    public static readonly Guid InProgress = Guid.Parse("f449c806-019b-48b6-a58d-1ba047ccc822");
    public static readonly Guid Interrupted = Guid.Parse("5a970db4-a244-449e-abe5-2500cd0d8a9a");
    public static readonly Guid Completed = Guid.Parse("4b10262f-4d4d-4075-a4fa-8dabcdedbeb1");
    public static readonly Guid TimedOut = Guid.Parse("470a5a87-8bf5-42e1-a350-22958764153d");
    public static readonly Guid Cancelled = Guid.Parse("7adf1e9d-24f7-4fff-a2ec-248d14a6254b");
}

[Table(nameof(TestAttemptStatus), Schema = Schemas.Lookup)]
public class TestAttemptStatus : LookupBase;

