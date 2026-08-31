using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class RoomStatusIds
{
    public static readonly Guid Active = Guid.Parse("4a728d18-ba82-4c00-a053-f95a6a638de4");
    public static readonly Guid Inactive = Guid.Parse("4e3a41f3-20d4-430b-a9fa-d175777b90ff");
    public static readonly Guid Maintenance = Guid.Parse("c588f1a1-3666-41f7-a826-0c0b32594243");
}

[Table(nameof(RoomStatus), Schema = Schemas.Lookup)]
public class RoomStatus : LookupBase;

