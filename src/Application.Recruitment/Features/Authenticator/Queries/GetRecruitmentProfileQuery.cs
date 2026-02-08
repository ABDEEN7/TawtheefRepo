using Application.Recruitment.Features.Authenticator.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Authenticator.Queries;

public record GetRecruitmentProfileQuery : IQuery<IResult<GetRecruitmentProfileDto>>
{
    public Guid UserId { get; init; }
    public string Language { get; init; } = "en";
}
