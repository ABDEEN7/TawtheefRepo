using Application.Recruitment.Features.Dashboard.DTOs;
using MediatR;
using FluentResults;

namespace Application.Recruitment.Features.Dashboard.Queries;

public sealed record GetCandidateInvitationStatisticsQuery(Guid UserId) : IRequest<IResult<CandidateInvitationStatisticsDto>>;

