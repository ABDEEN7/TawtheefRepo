using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class InterviewCommitteeTypeIds
{
    public static Guid Academic = Guid.Parse("8e6f2a10-6a1e-4b8a-9f0a-1a2b3c4d5e01");
    public static Guid Administrative = Guid.Parse("8e6f2a10-6a1e-4b8a-9f0a-1a2b3c4d5e02");
    public static Guid Labor = Guid.Parse("8e6f2a10-6a1e-4b8a-9f0a-1a2b3c4d5e03");
    public static Guid Other = Guid.Parse("8e6f2a10-6a1e-4b8a-9f0a-1a2b3c4d5e04");
}

[Table(nameof(InterviewCommitteeType), Schema = Schemas.Lookup)]
public class InterviewCommitteeType : LookupBase;
