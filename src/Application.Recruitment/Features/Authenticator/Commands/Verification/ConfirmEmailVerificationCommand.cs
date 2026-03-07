using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands.Verification;

public record ConfirmEmailVerificationCommand(Guid? UserId, string Email, string Code)
    : IRequest<IResult<Unit>>;

