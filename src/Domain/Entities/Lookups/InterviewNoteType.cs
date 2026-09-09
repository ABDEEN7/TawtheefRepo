using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class InterviewNoteTypeIds
{
    public static Guid ChairmanNote = Guid.Parse("8e6f2a12-6a1e-4b8a-9f0a-1a2b3c4d5e01");
    public static Guid HRNote = Guid.Parse("8e6f2a12-6a1e-4b8a-9f0a-1a2b3c4d5e02");
    public static Guid OperationalNote = Guid.Parse("8e6f2a12-6a1e-4b8a-9f0a-1a2b3c4d5e03");
}

[Table(nameof(InterviewNoteType), Schema = Schemas.Lookup)]
public class InterviewNoteType : LookupBase;
