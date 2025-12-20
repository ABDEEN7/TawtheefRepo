using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Operations.Employee.Job.DTOs;

public class JobTabReviewNoteResponseDto
{
    public Guid Id { get; set; }
    public Guid ReviewCycleId { get; set; }
    public string? Tab { get; set; }
    public string Note { get; set; } = string.Empty;
    public string? TabStatus { get; set; }

    public bool IsResolved { get; set; } = false;

    public List<FileRefDto> Attachments { get; set; } = [];
}
