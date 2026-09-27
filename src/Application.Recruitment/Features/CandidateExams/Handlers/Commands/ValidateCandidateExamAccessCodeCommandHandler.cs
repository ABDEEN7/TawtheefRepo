using Application.Recruitment.Features.CandidateExams.Commands;
using Application.Recruitment.Features.CandidateExams.DTOs;
using FluentResults;
using MediatR;
using Application.Recruitment.Features.CandidateExams.Services;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Commands;

public sealed class ValidateCandidateExamAccessCodeCommandHandler(CandidateExamEntryValidationService validationService)
    : IRequestHandler<ValidateCandidateExamAccessCodeCommand, IResult<ValidateCandidateExamAccessCodeResponse>>
{
    public async Task<IResult<ValidateCandidateExamAccessCodeResponse>> Handle(
        ValidateCandidateExamAccessCodeCommand command,
        CancellationToken cancellationToken)
    {
        var validation = await validationService.ValidateInitialEntryAsync(command.UserId,
            command.TestSessionCandidateId, command.AccessCode, cancellationToken);
        if (validation.IsFailed)
            return Result.Fail<ValidateCandidateExamAccessCodeResponse>(validation.Errors);

        return Result.Ok(new ValidateCandidateExamAccessCodeResponse(
            validation.Value.TestSessionCandidateId,
            validation.Value.TestSessionId,
            validation.Value.ExamId,
            CandidateExamEntryStates.ReadyToEnter));
    }
}
