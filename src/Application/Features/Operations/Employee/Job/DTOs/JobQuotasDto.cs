using System.ComponentModel.DataAnnotations;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

public class JobQuotaDto
{
    public Guid? Id { get; set; }
    public decimal QatariCitizens { get; set; }
    public decimal QatarMother { get; set; }
    public decimal NonQatariSpouse { get; set; }
    public decimal Gcc { get; set; }
    public decimal QuGrads { get; set; }
    public decimal Residents { get; set; }
    public List<ResidentBreakdownDto> ResidentsBreakdowns { get; set; } = [];
}
