using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Queries;

public sealed record GetCandidateExamActiveExecutionQuery(Guid UserId, Guid TestAttemptId)
    : IRequest<IResult<CandidateExamActiveExecutionResponse>>;
