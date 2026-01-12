using Microsoft.AspNetCore.Http;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs;

public sealed class FinalizeUserProfileReviewRequest
{
    public string? Notes { get; set; }
    public string? Summary { get; set; }
    public IFormFile? ExceptionalFile { get; set; }
}
