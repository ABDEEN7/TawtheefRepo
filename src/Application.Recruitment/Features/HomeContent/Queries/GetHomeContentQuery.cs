using Application.Recruitment.Features.HomeContent.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.HomeContent.Queries;

public sealed record GetHomeContentQuery : IQuery<IResult<HomeContentDto>>;
