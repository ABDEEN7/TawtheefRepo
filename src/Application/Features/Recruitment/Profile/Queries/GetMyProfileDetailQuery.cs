using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Queries;

public sealed record GetMyProfileDetailQuery : IRequest<Result<MyProfileDetailDto>>
{
    public Guid UserId { get; init; }
}
