using Application.Recruitment.Features.Profile.DTOs.SaveOperation;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Command.ChangeRequestOperation;

public sealed record RequestProfileEducationChangeCommand(
    Guid UserId,
    SaveProfileEducationRequest Request
) : IRequest<IResult<Unit>>;


