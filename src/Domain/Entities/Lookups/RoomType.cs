using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class RoomTypeIds
{
    public static readonly Guid ExamRoom = Guid.Parse("aa2ccf78-fff4-4b2c-a843-1d88f6029c68");
    public static readonly Guid InterviewRoom = Guid.Parse("fd3c28b0-60f4-4b4e-a61a-a148090691a2");
    public static readonly Guid LabRoom = Guid.Parse("268c28c4-5ab5-4f2a-a050-901a46da9b86");
    public static readonly Guid Other = Guid.Parse("779f2dce-f563-492a-a434-44f24dd2d0ae");
}

[Table(nameof(RoomType), Schema = Schemas.Lookup)]
public class RoomType : LookupBase;

