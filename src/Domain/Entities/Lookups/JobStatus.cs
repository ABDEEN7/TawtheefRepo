using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class JobStatusIds
{
    public static readonly Guid Draft     = Guid.Parse("8C6D6C8C-9F68-471A-9F07-0C2B16D4E101");
    public static readonly Guid Active    = Guid.Parse("6CFC1E9C-9B2B-4D20-B7B1-7A4B2C0A41D4");
    public static readonly Guid Closed    = Guid.Parse("B1B2C364-7B10-46B6-8B4B-9D15E2CE2A23");
    public static readonly Guid Cancelled = Guid.Parse("0B3E5E28-0C0A-4C0C-9B6B-0AF1A7F0B5C8");
}
[Table(nameof(JobStatus), Schema = Schemas.Lookup)]
public class JobStatus : LookupBase
{
}
