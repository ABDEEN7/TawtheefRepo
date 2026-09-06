using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestSessionCandidateIdentityVerificationStatusIds
{
    public static readonly Guid Pending = Guid.Parse("d086861a-62a7-4f66-a014-b4245da3ff82");
    public static readonly Guid Matched = Guid.Parse("58e27acd-91c7-4b11-a874-c28daa7916a5");
    public static readonly Guid NotMatched = Guid.Parse("bcd7322c-f79d-41ba-a3dd-00407fc984a6");
}

[Table(nameof(TestSessionCandidateIdentityVerificationStatus), Schema = Schemas.Lookup)]
public class TestSessionCandidateIdentityVerificationStatus : LookupBase;

