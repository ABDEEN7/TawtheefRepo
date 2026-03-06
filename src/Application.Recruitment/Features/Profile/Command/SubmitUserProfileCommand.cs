using Application.Recruitment.Features.Profile.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command;


public sealed record SubmitUserProfileCommand(
    Guid UserId,
    SubmitUserProfileRequest Request
) : IRequest<IResult<Unit>>;

