using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands;

public sealed record RequestQatarResidentOtpCommand(string Qid, string PhoneNumber, DateOnly QidExpiry)
    : ICommand<IResult<Unit>>;
