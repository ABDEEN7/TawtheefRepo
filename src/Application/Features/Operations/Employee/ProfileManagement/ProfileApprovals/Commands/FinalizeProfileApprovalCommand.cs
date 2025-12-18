using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record FinalizeProfileApprovalCommand(
    Guid OfficerId,
    Guid UserProfileId,
    FinalApprovalAction Action,
    string? Notes,
    string? Summary,
    IReadOnlyCollection<Guid> NeedsCorrectionItems,
    IFormFile? RejectionDocument,
    IFormFile? ExceptionalFile,
    bool HasManagerOverride
) : IRequest<IResult<Unit>>;
