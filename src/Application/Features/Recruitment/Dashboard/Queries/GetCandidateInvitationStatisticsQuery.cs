using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Queries;

public sealed record GetCandidateInvitationStatisticsQuery(Guid UserId) : IRequest<IResult<CandidateInvitationStatisticsDto>>;
