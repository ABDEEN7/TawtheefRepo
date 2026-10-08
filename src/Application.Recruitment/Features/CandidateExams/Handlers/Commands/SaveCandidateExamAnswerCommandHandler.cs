using Application.Recruitment.Features.CandidateExams.Commands;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Services;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Commands;

public sealed class SaveCandidateExamAnswerCommandHandler(CandidateExamExecutionMutationService service)
    : IRequestHandler<SaveCandidateExamAnswerCommand, IResult<SaveCandidateExamAnswerResponse>>
{
    public Task<IResult<SaveCandidateExamAnswerResponse>> Handle(SaveCandidateExamAnswerCommand command,
        CancellationToken cancellationToken) => service.SaveAnswerAsync(command.UserId, command.TestAttemptId,
        command.ActiveSessionId,
        command.TestAttemptQuestionId, command.SelectedOptionId, cancellationToken);
}
