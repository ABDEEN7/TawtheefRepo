using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class ResidentBreakdownDto
{    
    public Guid? Id { get; set; }
    public Guid NationalityId { get; set; }
    public decimal Percentage { get; set; }
}
