using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Features.Recruitment.Dashboard.DTOs;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Queries;

public sealed record GetCandidateInvitationStatisticsQuery(Guid UserId) : IQuery<IResult<CandidateInvitationStatisticsDto>>;
