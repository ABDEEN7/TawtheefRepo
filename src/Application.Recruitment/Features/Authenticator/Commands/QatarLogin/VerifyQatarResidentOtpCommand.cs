using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Application.Recruitment.Features.Authenticator.Commands.QatarLogin;

public sealed record VerifyQatarResidentOtpCommand(string Qid, string PhoneNumber, DateOnly QidExpiry, string Otp)
    : IRequest<IResult<AuthResponse>>;
