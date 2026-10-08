using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Commands;

public sealed record SaveCandidateExamAnswerCommand(
    Guid UserId,
    Guid TestAttemptId,
    Guid TestAttemptQuestionId,
    Guid ActiveSessionId,
    Guid SelectedOptionId) : IRequest<IResult<SaveCandidateExamAnswerResponse>>;
