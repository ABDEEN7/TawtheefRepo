using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record VerifyOtpCommand(string Email, string Otp) : IRequest<Result<AuthResponse>>;