using Application.Recruitment.Features.CandidateExams.Common;
using Application.Recruitment.Features.CandidateExams.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;

namespace Application.Recruitment.Features.CandidateExams.Services;

public sealed class CandidateExamPartTransitionContextService(IUnitOfWork unitOfWork)
{
    internal async Task<IResult<CandidateExamPartTransitionContext>> LoadAsync(Guid userId, Guid attemptId,
        Guid requestedPartId, CancellationToken cancellationToken)
    {
        var attemptState = await unitOfWork.Context.Set<TestAttempt>()
            .OwnedByApplicant(userId)
            .Where(x => x.Id == attemptId)
            .Select(x => new
            {
                Attempt = x,
                Candidate = x.TestSessionCandidate!,
                SessionStatusId = x.TestSessionCandidate!.TestSession!.StatusId,
                SlotStatusId = x.TestSessionCandidate.TestSession!.TestSlot!.StatusId,
                x.TestSessionCandidate.TestSession!.Exam!.AllowPreviousQuestion
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (attemptState is null)
        {
            return Failure(ErrorsCodes.CandidateExamAttemptNotFound, StatusCodes.Status404NotFound);
        }

        var parts = await unitOfWork.Context.Set<TestAttemptPart>()
            .Where(x => x.TestAttemptId == attemptId)
            .OrderBy(x => x.ExamPart!.PartNo)
            .Select(x => new CandidateExamPartTransitionData(x, x.ExamPart!.PartNo, x.ExamPart.DurationMinutes,
                x.ExamPart.QualificationScore))
            .ToListAsync(cancellationToken);

        var requestedIndex = parts.FindIndex(x => x.Part.Id == requestedPartId);

        if (requestedIndex < 0)
        {
            return Failure(ErrorsCodes.CandidateExamPartNotCurrent, StatusCodes.Status404NotFound);
        }

        return Result.Ok(new CandidateExamPartTransitionContext(attemptState.Attempt, attemptState.Candidate,
            attemptState.SessionStatusId, attemptState.SlotStatusId, attemptState.AllowPreviousQuestion, parts,
            requestedIndex));
    }

    private static IResult<CandidateExamPartTransitionContext> Failure(string code,
        int statusCode = StatusCodes.Status409Conflict) =>
        Result.Fail<CandidateExamPartTransitionContext>(new Error(code).WithMetadata("Code", code)
            .WithMetadata("StatusCode", statusCode));
}
