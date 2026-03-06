using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands.Verification;

public record RequestEmailVerificationCommand(Guid? UserId, string Email)
    : IRequest<IResult<Unit>>;

