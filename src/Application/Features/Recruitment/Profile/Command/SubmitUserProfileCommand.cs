using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Command;


public sealed record SubmitUserProfileCommand(
    Guid UserId,
    SubmitUserProfileRequest Request
) : IRequest<IResult<Unit>>;
