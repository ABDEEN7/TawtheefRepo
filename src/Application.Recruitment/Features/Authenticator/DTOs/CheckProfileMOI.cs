using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Application.Recruitment.Features.Authenticator.DTOs;

public record CheckProfileMOI(string QID, DateOnly ExpiryDate)
    : IRequest<IResult<MOEPersonalInfo>>;

