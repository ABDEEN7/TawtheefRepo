using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Commands;

public sealed record ValidateCandidateExamAccessCodeCommand(
    Guid UserId,
    Guid TestSessionCandidateId,
    string AccessCode) : IRequest<IResult<ValidateCandidateExamAccessCodeResponse>>;
