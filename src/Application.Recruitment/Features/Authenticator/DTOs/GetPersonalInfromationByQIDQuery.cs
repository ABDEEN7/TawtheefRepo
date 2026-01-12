using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.DTOs;

public record GetPersonalInformationByQidQuery(Guid UserId, CheckProfileMOI Request): IQuery<IResult<MOEPersonalInfo>>;
