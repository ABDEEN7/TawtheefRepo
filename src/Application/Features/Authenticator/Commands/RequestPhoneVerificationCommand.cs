using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RequestPhoneVerificationCommand(Guid? UserId, string PhoneE164)
    : IRequest<IResult<Unit>>;
