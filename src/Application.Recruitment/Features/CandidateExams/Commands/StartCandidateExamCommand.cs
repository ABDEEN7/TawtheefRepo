using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Commands;

public sealed record StartCandidateExamCommand(
    Guid UserId,
    Guid TestSessionCandidateId,
    string AccessCode,
    Guid ActiveSessionId)
    : IRequest<IResult<StartCandidateExamResponse>>;
