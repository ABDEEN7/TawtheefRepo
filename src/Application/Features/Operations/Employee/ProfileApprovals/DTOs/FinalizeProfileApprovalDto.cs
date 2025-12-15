using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileApprovals.DTOs;

public sealed record FinalizeProfileApprovalDto
{
    public Guid OfficerId { get; init; }

    public Guid UserProfileId { get; init; }

    public FinalApprovalAction Action { get; init; }

    public string? Notes { get; init; }

    public string? Summary { get; init; }

    public IReadOnlyCollection<Guid> NeedsCorrectionItems { get; init; } = Array.Empty<Guid>();

    public IFormFile? RejectionDocument { get; init; }

    public IFormFile? ExceptionalFile { get; init; }

    public bool HasManagerOverride { get; init; }
}
