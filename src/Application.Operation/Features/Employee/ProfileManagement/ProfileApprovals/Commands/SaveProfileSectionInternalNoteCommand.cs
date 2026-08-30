using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.Commands;

public sealed record SaveProfileSectionInternalNoteCommand(
    Guid OfficerId,
    Guid UserProfileId,
    ProfileSection Section,
    string? Note) : IRequest<IResult<Unit>>;
