using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.RevisionOperation;

public sealed record ReviseProfileContactCommand(
    Guid UserId,
    SaveProfileContactRequest Request
) : IRequest<IResult<Unit>>;

