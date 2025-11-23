namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public record JobQuotasDto
{
    public decimal QatariCitizens { get; init; }
    public decimal QatarMother { get; init; }
    public decimal NonQatariSpouse { get; init; }
    public decimal Gcc { get; init; }
    public decimal QuGrads { get; init; }
    public decimal Residents { get; init; }
    public List<ResidentBreakdownDto> ResidentsBreakdown { get; init; } = [];
}

