using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class ManagmentIds
{
    public static readonly Guid Minister =  Guid.Parse("4575068a-4f0d-b6cd-8ea8-be680d8dc992");
    public static readonly Guid TrainingCenter =  Guid.Parse("453aa49d-8f16-a85b-9991-c8355ac8bf00");
}

[Table(nameof(Managment), Schema = Schemas.Lookup)]
public class Managment : LookupBase
{
    public Guid SectorId { get; set; }
    public Sector? Sector { get; set; }

    public ICollection<Department> Departments { get; set; } = [];
}
