using Tawtheef.Application.Common.Models;

namespace Application.Recruitment.Features.JobDetails.DTOs;

public class JobSpecializationResponseDto
{
    public Guid Id { get; set; }
    public DropdownOptions Major { get; set; } = null!;
    public DropdownOptions? SubMajor { get; set; }
}
