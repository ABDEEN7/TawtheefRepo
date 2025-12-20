using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record DecideProfileSectionCommand(
    Guid OfficerId,
    Guid UserProfileId,
    ProfileSection Section,
    ReviewStatus Status, // Approved أو NeedsCorrection
    string? Note
) : IRequest<IResult<Unit>>;
