namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobQuotaResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public required decimal QatariCitizens { get; set; }
    public required decimal QatarMother { get; set; }
    public required decimal NonQatariSpouse { get; set; }
    public required decimal Gcc { get; set; }
    public required decimal QuGrads { get; set; }
    public required decimal Residents { get; set; }
    public List<ResidentBreakdownResponseDto>? ResidentsBreakdowns { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
