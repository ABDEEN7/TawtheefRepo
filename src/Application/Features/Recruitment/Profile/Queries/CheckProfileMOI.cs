using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Queries;

public record CheckProfileMOI(string QID, DateOnly ExpiryDate): IQuery<IResult<MOEPersonalInfo>>;
