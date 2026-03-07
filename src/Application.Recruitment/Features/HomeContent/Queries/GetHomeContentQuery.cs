using Application.Recruitment.Features.HomeContent.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.HomeContent.Queries;

public sealed record GetHomeContentQuery : IRequest<IResult<HomeContentDto>>;

