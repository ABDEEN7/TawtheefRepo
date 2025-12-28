using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public sealed class SaveJobReviewRequestDto
{
    public string? AttachmentsJson { get; set; }
    public List<JobTabReviewUpsertDto> Tabs { get; set; } = [];
    public List<IFormFile> Files { get; set; } = [];
}
