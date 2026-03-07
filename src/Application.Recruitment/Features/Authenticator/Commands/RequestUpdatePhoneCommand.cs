using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands;

public record RequestUpdatePhoneCommand(Guid? UserId, string PhoneE164)
    : IRequest<IResult<Unit>>;

