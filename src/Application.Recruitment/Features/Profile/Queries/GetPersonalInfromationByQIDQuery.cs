using Application.Recruitment.Features.Authenticator.DTOs;
using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Application.Recruitment.Features.Profile.Queries;

public record GetPersonalInformationByQidQuery(CheckProfileMOI Request)
    : IRequest<IResult<MOEPersonalInfo>>;

