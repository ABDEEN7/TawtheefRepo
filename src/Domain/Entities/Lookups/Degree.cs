using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class DegreeIds
{
    public static readonly Guid Doctorate = Guid.Parse("d60cb9b1-f0ce-4c0a-a147-51e8d3947ef4");
    public static readonly Guid Master = Guid.Parse("8ca14478-019d-4d3e-89cd-90cb89baf463");
    public static readonly Guid Bachelor = Guid.Parse("de5901db-60dd-49f0-953c-daf923cf9f4a");
    public static readonly Guid Diploma = Guid.Parse("f6249ce2-fa02-4e18-9c89-151ba3be0c12");
    public static readonly Guid Secondary = Guid.Parse("ebf2faa1-5ce6-4a04-9472-2746bfbbd252");
    public static readonly Guid Preparatory = Guid.Parse("f1a31fe6-ba80-46cb-b24b-f402bcb4fdec");
    public static readonly Guid Elementary = Guid.Parse("6e453f48-5f2f-4f98-8b76-f416cdd4811b");
}

[Table(nameof(Degree), Schema = Schemas.Lookup)]
public class Degree : LookupBase
{
    
}
