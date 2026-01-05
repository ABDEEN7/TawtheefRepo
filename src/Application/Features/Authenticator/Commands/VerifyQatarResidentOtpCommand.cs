using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public sealed record VerifyQatarResidentOtpCommand(string Qid, string PhoneNumber, DateOnly QidExpiry, string Otp)
    : IRequest<IResult<AuthResponse>>;
