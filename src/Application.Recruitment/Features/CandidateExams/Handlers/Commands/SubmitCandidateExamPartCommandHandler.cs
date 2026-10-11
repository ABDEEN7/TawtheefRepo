using Application.Recruitment.Features.CandidateExams.Commands;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Services;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Commands;

public sealed class SubmitCandidateExamPartCommandHandler(CandidateExamExecutionMutationService service)
    : IRequestHandler<SubmitCandidateExamPartCommand, IResult<SubmitCandidateExamPartResponse>>
{
    public Task<IResult<SubmitCandidateExamPartResponse>> Handle(SubmitCandidateExamPartCommand command,
        CancellationToken cancellationToken) => service.SubmitPartAsync(command.UserId,
        command.TestAttemptId, command.ActiveSessionId, command.TestAttemptPartId, cancellationToken);
}
