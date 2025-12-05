using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Queries;

public record CheckProfileMOI(string QID, DateOnly ExpiryDate): IRequest<IResult<MOEPersonalInfo>>;
