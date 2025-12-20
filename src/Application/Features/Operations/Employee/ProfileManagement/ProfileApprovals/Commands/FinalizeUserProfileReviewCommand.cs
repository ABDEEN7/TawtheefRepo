using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record FinalizeUserProfileReviewCommand(
    Guid OfficerId,
    Guid UserProfileId,
    string? Notes,
    string? Summary
) : IRequest<IResult<Unit>>;
