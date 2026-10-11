using Application.Recruitment.Features.CandidateExams.Commands;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Services;
using FluentResults;
using MediatR;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Commands;

public sealed class NavigateCandidateExamCommandHandler(CandidateExamExecutionMutationService service)
    : IRequestHandler<NavigateCandidateExamCommand, IResult<NavigateCandidateExamResponse>>
{
    public Task<IResult<NavigateCandidateExamResponse>> Handle(NavigateCandidateExamCommand command,
        CancellationToken cancellationToken) => service.NavigateAsync(command.UserId, command.TestAttemptId,
        command.ActiveSessionId,
        command.TargetQuestionNumber, cancellationToken);
}
