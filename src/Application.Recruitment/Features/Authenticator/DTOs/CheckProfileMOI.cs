using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.DTOs;

public record CheckProfileMOI(string QID, DateOnly ExpiryDate): IQuery<IResult<Tawtheef.Application.Features.Authenticator.DTOs.MOEPersonalInfo>>;
