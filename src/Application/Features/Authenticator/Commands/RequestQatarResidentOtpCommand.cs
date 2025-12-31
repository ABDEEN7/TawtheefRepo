using FluentResults;
using MediatR;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public sealed record RequestQatarResidentOtpCommand(string Qid, string PhoneNumber)
    : IRequest<IResult<Unit>>;
