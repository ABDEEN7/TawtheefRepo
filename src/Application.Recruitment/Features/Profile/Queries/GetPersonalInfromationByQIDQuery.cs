using Application.Recruitment.Features.Profile.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Queries;

public record GetPersonalInformationByQidQuery(Guid UserId, CheckProfileMOI Request): IQuery<IResult<MOEPersonalInfo>>;
