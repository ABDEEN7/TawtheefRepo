using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Commands;

public sealed record AgreeToTermsCommand(Guid UserId) : IRequest<IResult<Unit>>;

