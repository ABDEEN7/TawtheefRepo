using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Queries;

public sealed record GetCandidateExamCurrentQuestionQuery(Guid UserId, Guid ActiveSessionId, Guid TestAttemptId)
    : IRequest<IResult<CandidateExamCurrentQuestionResponse>>;
