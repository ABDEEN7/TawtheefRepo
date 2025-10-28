using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Applicant;

public class ResidenceAddress : EventEntity
{
    public required string Address { get; set; }
    public int? BuildingNo { get; set; }
    public int? StreetNo { get; set; }
    public int? ZoneNo { get; set; }
    public int? UnitNo { get; set; }
    
    ResidenceAddress Create(string? address, int? buildingNo, int? streetNo, int? zoneNo, int? unitNo)
    {
        BuildingNo = buildingNo;
        StreetNo = streetNo;
        ZoneNo = zoneNo;
        UnitNo = unitNo; 
        Address = address ?? string.Join("-", [buildingNo, streetNo, zoneNo, unitNo]);
        return this;
    }
}
