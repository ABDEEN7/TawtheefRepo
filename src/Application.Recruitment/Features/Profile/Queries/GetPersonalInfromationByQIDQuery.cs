using Application.Recruitment.Features.Authenticator.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Application.Recruitment.Features.Profile.Queries;

public record GetPersonalInformationByQidQuery(CheckProfileMOI Request)
    : IQuery<IResult<MOEPersonalInfo>>;
