using Application.Recruitment.Features.CandidateExams.Commands;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Services;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Commands;

public sealed class TimeoutCandidateExamPartCommandHandler(CandidateExamExecutionMutationService service)
    : IRequestHandler<TimeoutCandidateExamPartCommand, IResult<TimeoutCandidateExamPartResponse>>
{
    public Task<IResult<TimeoutCandidateExamPartResponse>> Handle(TimeoutCandidateExamPartCommand command,
        CancellationToken cancellationToken) => service.TimeoutPartAsync(command.UserId,
        command.TestAttemptId,command.ActiveSessionId, command.TestAttemptPartId, cancellationToken);
}
