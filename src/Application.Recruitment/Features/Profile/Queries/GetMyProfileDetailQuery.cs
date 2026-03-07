using Application.Recruitment.Features.Profile.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Queries;

public sealed record GetMyProfileDetailQuery : IRequest<Result<MyProfileDetailDto>>
{
    public Guid UserId { get; init; }
}

