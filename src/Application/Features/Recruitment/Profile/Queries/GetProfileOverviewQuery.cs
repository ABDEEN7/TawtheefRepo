using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Profile.Queries;

public sealed record GetProfileOverviewQuery(Guid UserId) : IRequest<Result<ProfileOverviewDto>>;
