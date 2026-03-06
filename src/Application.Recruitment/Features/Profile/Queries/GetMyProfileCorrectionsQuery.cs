using Application.Recruitment.Features.Profile.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Queries;

public sealed record GetMyProfileCorrectionsQuery(Guid UserId)
    : IRequest<Result<ProfileCorrectionsDto>>;

