using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Commands;

public sealed record NavigateCandidateExamCommand(
    Guid UserId,
    Guid TestAttemptId,
    Guid ActiveSessionId,
    int TargetQuestionNumber)
    : IRequest<IResult<NavigateCandidateExamResponse>>;
