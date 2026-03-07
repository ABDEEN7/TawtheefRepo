using Application.Recruitment.Features.Authenticator.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Queries;

public record GetRecruitmentProfileQuery : IRequest<IResult<GetRecruitmentProfileDto>>
{
    public Guid UserId { get; init; }
    public string Language { get; init; } = "en";
}

