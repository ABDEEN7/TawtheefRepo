using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record ConfirmPhoneVerificationCommand(Guid? UserId, string PhoneE164, string Code)
    : IRequest<IResult<Unit>>;
