using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands.QatarLogin;

public sealed record RequestQatarResidentOtpCommand(string Qid, string PhoneNumber, DateOnly QidExpiry)
    : IRequest<IResult<Unit>>;

