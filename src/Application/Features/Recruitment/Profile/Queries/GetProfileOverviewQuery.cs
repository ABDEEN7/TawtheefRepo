using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Queries;

public sealed record GetProfileOverviewQuery(Guid UserId) : IQuery<Result<ProfileOverviewDto>>;
