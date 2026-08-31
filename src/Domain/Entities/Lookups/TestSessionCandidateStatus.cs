using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestSessionCandidateStatusIds
{
    public static readonly Guid Assigned = Guid.Parse("ec074f24-591a-41e4-ac6a-4b186c6043b0");
    public static readonly Guid Authorized = Guid.Parse("29747ce1-e3b6-45ff-a740-59730f5e5833");
    public static readonly Guid Started = Guid.Parse("c8a60391-f3b2-4afd-a0e3-15492812b141");
    public static readonly Guid Completed = Guid.Parse("b60c31b5-b819-4cf3-ae25-fd9d075b4db7");
    public static readonly Guid Rescheduled = Guid.Parse("213abc60-4724-4f6c-a939-04eaf4deef78");
    public static readonly Guid Cancelled = Guid.Parse("ca2ae8f7-8fc3-4c7d-ae0c-872b4e13b451");
}

[Table(nameof(TestSessionCandidateStatus), Schema = Schemas.Lookup)]
public class TestSessionCandidateStatus : LookupBase;

