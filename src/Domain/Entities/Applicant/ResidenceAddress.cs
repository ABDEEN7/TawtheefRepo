using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(ResidenceAddress), Schema = Schemas.Profile)]
public class ResidenceAddress : EventEntity
{
    public int BuildingNo { get; set; }
    public int StreetNo { get; set; }
    public int ZoneNo { get; set; }
    public int UnitNo { get; set; }
    
    public static ResidenceAddress Create(int buildingNo, int streetNo, int zoneNo, int unitNo)
    {
        return new ResidenceAddress
        {
            BuildingNo = buildingNo,
            StreetNo = streetNo,
            ZoneNo = zoneNo,
            UnitNo = unitNo
        };
    }
}
