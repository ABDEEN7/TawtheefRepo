using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Commands;

public sealed record TimeoutCandidateExamPartCommand(
    Guid UserId,
    Guid TestAttemptId,
    Guid ActiveSessionId,
    Guid TestAttemptPartId)
    : IRequest<IResult<TimeoutCandidateExamPartResponse>>;
