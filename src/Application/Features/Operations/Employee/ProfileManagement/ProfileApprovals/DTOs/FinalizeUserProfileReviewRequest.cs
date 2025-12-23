using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

public sealed class FinalizeUserProfileReviewRequest
{
    public string? Notes { get; set; }
    public string? Summary { get; set; }
    public IFormFile? ExceptionalFile { get; set; }
}
