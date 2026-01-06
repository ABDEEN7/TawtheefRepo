using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Operations.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record DecideProfileSectionCommand(
    Guid OfficerId,
    Guid UserProfileId,
    ProfileSection Section,
    ReviewStatus Status, // Approved ?? NeedsCorrection
    string? Note
) : ICommand<IResult<Unit>>;
