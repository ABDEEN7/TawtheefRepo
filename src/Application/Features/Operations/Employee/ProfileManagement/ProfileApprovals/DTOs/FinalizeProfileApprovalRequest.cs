using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.DTOs;

public sealed class FinalizeProfileApprovalRequest
{
    public FinalApprovalAction Action { get; set; }
    public string? Notes { get; set; }
    public string? Summary { get; set; }
    public IReadOnlyCollection<Guid>? NeedsCorrectionItems { get; set; }
    public IFormFile? RejectionDocument { get; set; }
    public IFormFile? ExceptionalFile { get; set; }
}
