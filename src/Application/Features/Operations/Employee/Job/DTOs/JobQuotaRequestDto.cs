using System.ComponentModel.DataAnnotations;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

public class JobQuotaRequestDto
{
    [Range(0, 100, ErrorMessage = JobValidationMessages.QUOTA_PERCENTAGE_RANGE)]
    public decimal QatariCitizens { get; set; }

    [Range(0, 100, ErrorMessage = JobValidationMessages.QUOTA_PERCENTAGE_RANGE)]
    public decimal QatarMother { get; set; }

    [Range(0, 100, ErrorMessage = JobValidationMessages.QUOTA_PERCENTAGE_RANGE)]
    public decimal NonQatariSpouse { get; set; }

    [Range(0, 100, ErrorMessage = JobValidationMessages.QUOTA_PERCENTAGE_RANGE)]
    public decimal Gcc { get; set; }

    [Range(0, 100, ErrorMessage = JobValidationMessages.QUOTA_PERCENTAGE_RANGE)]
    public decimal QuGrads { get; set; }

    [Range(0, 100, ErrorMessage = JobValidationMessages.QUOTA_PERCENTAGE_RANGE)]
    public decimal Residents { get; set; }

    public List<ResidentBreakdownRequestDto>? ResidentsBreakdowns { get; set; }
}
