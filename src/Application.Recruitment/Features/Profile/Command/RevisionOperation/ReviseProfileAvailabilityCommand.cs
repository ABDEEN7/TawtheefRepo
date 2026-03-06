using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileAvailabilityCommand(
    Guid UserId,
    SaveProfileAvailabilityRequest Request
) : IRequest<IResult<Unit>>;

