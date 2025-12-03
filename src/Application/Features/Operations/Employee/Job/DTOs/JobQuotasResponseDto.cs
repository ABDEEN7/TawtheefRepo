namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobQuotaResponseDto
{
    public decimal QatariCitizens { get; set; }
    public decimal QatarMother { get; set; }
    public decimal NonQatariSpouse { get; set; }
    public decimal Gcc { get; set; }
    public decimal QuGrads { get; set; }
    public decimal Residents { get; set; }
    public List<ResidentBreakdownResponseDto> ResidentsBreakdowns { get; set; } = new();
}
