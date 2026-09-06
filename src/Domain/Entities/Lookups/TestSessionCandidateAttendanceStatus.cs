using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class TestSessionCandidateAttendanceStatusIds
{
    public static readonly Guid Pending = Guid.Parse("5fec8448-15bc-406e-a70c-7d90be7fd362");
    public static readonly Guid Present = Guid.Parse("a3250ce0-5679-43fa-a257-9f545507fa5e");
    public static readonly Guid NoShow = Guid.Parse("5f4bc80b-ca7f-4f55-a394-cc4102007a39");
}

[Table(nameof(TestSessionCandidateAttendanceStatus), Schema = Schemas.Lookup)]
public class TestSessionCandidateAttendanceStatus : LookupBase;

