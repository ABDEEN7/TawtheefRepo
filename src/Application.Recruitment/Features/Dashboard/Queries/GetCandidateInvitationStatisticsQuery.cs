using Application.Recruitment.Features.Dashboard.DTOs;
using Cortex.Mediator.Queries;
using FluentResults;

namespace Application.Recruitment.Features.Dashboard.Queries;

public sealed record GetCandidateInvitationStatisticsQuery(Guid UserId) : IQuery<IResult<CandidateInvitationStatisticsDto>>;
