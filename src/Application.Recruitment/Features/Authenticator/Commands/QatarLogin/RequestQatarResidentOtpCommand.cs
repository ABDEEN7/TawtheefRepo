using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.Authenticator.Commands.QatarLogin;

public sealed record RequestQatarResidentOtpCommand(string Qid, string PhoneNumber, DateOnly QidExpiry, string? Language = "ar")
    : IRequest<IResult<Unit>>;

