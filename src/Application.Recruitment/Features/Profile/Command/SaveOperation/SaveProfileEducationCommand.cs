using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.SaveOperation;

public sealed record SaveProfileEducationCommand(
    Guid UserId,
    SaveProfileEducationRequest Request
) : IRequest<IResult<Unit>>;

