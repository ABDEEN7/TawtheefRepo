using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class SchoolStageIds
{
    public static readonly Guid Kindergarten = Guid.Parse("5c1a7e10-3b2d-4f6a-9c01-7d2e4b8a0001");
    public static readonly Guid Primary = Guid.Parse("5c1a7e10-3b2d-4f6a-9c01-7d2e4b8a0002");
    public static readonly Guid Preparatory = Guid.Parse("5c1a7e10-3b2d-4f6a-9c01-7d2e4b8a0003");
    public static readonly Guid Secondary = Guid.Parse("5c1a7e10-3b2d-4f6a-9c01-7d2e4b8a0004");
}

// Committee Head's optional "recommended school stage" per candidate - informational only, never
// part of any score, qualification or suggestion calculation.
[Table(nameof(SchoolStage), Schema = Schemas.Lookup)]
public class SchoolStage : LookupBase;
