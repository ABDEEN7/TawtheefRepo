using Application.Recruitment.Features.Profile.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Profile.Queries;

public sealed record GetProfileOverviewQuery(Guid UserId) : IQuery<Result<ProfileOverviewDto>>;
