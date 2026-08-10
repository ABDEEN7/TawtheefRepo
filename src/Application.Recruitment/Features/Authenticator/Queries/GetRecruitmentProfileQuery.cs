using Application.Recruitment.Features.Authenticator.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Queries;

public record GetRecruitmentProfileQuery(Guid UserId, string Language = "en") 
    : IRequest<IResult<GetRecruitmentProfileDto>>;

