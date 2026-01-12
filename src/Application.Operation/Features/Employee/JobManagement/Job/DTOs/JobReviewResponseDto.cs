using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Application.Operation.Features.Employee.Job.DTOs;

public class JobReviewResponseDto
{
    public List<JobTabReviewNoteResponseDto>? TabNoteReviews { get; set; }
    public FileRefDto? ReviewAttachment { get; set; }
}
