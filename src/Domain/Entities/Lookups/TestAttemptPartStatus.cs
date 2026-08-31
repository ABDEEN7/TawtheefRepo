using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestAttemptPartStatusIds
{
    public static readonly Guid NotStarted = Guid.Parse("2e520f0b-1fe3-4d5d-aa03-2d192de69599");
    public static readonly Guid InProgress = Guid.Parse("07c2f269-6135-40f9-a146-6957271a4415");
    public static readonly Guid Completed = Guid.Parse("cee388fa-585f-480a-ac6f-6688143ff92e");
    public static readonly Guid TimedOut = Guid.Parse("6a32c2bc-11ca-4834-a3ed-3eaa7f622f50");
    public static readonly Guid Interrupted = Guid.Parse("251ec109-529e-49c3-a3c0-626b974b5d37");
}

[Table(nameof(TestAttemptPartStatus), Schema = Schemas.Lookup)]
public class TestAttemptPartStatus : LookupBase;

