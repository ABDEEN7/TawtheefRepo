using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;

public sealed record SaveProfileAvailabilityCommand(
    Guid UserId,
    SaveProfileAvailabilityRequest Request
) : IRequest<IResult<Unit>>;

