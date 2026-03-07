using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands.Verification;

public record RequestPhoneVerificationCommand(Guid? UserId, string PhoneE164)
    : IRequest<IResult<Unit>>;

