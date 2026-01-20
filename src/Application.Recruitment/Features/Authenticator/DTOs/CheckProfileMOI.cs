using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Application.Recruitment.Features.Authenticator.DTOs;

public record CheckProfileMOI(string QID, DateOnly ExpiryDate)
    : IQuery<IResult<MOEPersonalInfo>>;
