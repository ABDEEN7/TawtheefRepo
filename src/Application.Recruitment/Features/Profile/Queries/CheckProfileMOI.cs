using Application.Recruitment.Features.Profile.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Queries;

public record CheckProfileMOI(string QID, DateOnly ExpiryDate): IQuery<IResult<MOEPersonalInfo>>;
