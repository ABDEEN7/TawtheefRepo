using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;

public sealed record SaveProfilePersonalCommand(
    Guid UserId,
    SaveProfilePersonalRequest Request
) : IRequest<IResult<Unit>>;

