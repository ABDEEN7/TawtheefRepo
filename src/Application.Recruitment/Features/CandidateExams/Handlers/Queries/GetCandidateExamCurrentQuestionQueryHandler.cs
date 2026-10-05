using Application.Recruitment.Features.CandidateExams.Common;
using Application.Recruitment.Features.CandidateExams.DTOs;
using Application.Recruitment.Features.CandidateExams.Models;
using Application.Recruitment.Features.CandidateExams.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Handlers.Queries;

public sealed class GetCandidateExamCurrentQuestionQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService,
    IMediaUrlResolver mediaUrlResolver,
    TimeProvider timeProvider)
    : IRequestHandler<GetCandidateExamCurrentQuestionQuery, IResult<CandidateExamCurrentQuestionResponse>>
{
    public async Task<IResult<CandidateExamCurrentQuestionResponse>> Handle(GetCandidateExamCurrentQuestionQuery query,
        CancellationToken cancellationToken)
    {
        var isArabic = localizationService.GetCurrentLanguage() == "ar";
        var attempt = await unitOfWork.Context.Set<TestAttempt>()
            .AsNoTracking()
            .Where(x => x.Id == query.TestAttemptId && x.TestSessionCandidate!.Invitation!.ApplicantId == query.UserId)
            .Select(x => new CandidateExamCurrentQuestionAttemptData(x.Id, x.StatusId, x.TestSessionCandidate!.StatusId,
                x.TestSessionCandidate.TestSession!.ExamId,
                x.TestSessionCandidate.TestSession.Exam!.AllowPreviousQuestion,
                x.TestSessionCandidate.TestSession.StatusId, x.ActiveSessionId,
                x.TestSessionCandidate.TestSession.TestSlot!.StatusId,
                isArabic
                    ? x.TestSessionCandidate.Invitation!.Job!.JobTitle!.JobNameAr
                    : x.TestSessionCandidate.Invitation!.Job!.JobTitle!.JobNameEn))
            .SingleOrDefaultAsync(cancellationToken);

        if (attempt is null)
            return Failure(ErrorsCodes.CandidateExamAttemptNotFound, StatusCodes.Status404NotFound);
        if (attempt.StatusId != TestAttemptStatusIds.InProgress)
            return Failure(CandidateExamAttemptStatusErrors.Resolve(attempt.StatusId));
        if (attempt.ActiveSessionId != query.ActiveSessionId)
            return Failure(ErrorsCodes.CandidateExamActiveSessionInvalid);
        if (attempt.CandidateStatusId != TestSessionCandidateStatusIds.Started)
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);
        if (attempt.SlotStatusId != TestSlotStatusIds.Started)
            return Failure(ErrorsCodes.CandidateExamSlotNotRunning);
        if (attempt.SessionStatusId != TestSessionStatusIds.InProgress)
            return Failure(ErrorsCodes.CandidateExamSessionNotRunning);

        var parts = await unitOfWork.Context.Set<TestAttemptPart>()
            .AsNoTracking()
            .Where(x => x.TestAttemptId == query.TestAttemptId)
            .Select(x => new CandidateExamCurrentQuestionPartData(x.Id, x.StatusId, x.ExamPart!.PartNo,
                isArabic ? x.ExamPart.TitleAr : x.ExamPart.TitleEn ?? x.ExamPart.TitleAr, x.StartedAt,
                x.DurationMinutesSnapshot, x.EffectiveDeadlineAt, x.LastQuestionNo, x.Questions.Count))
            .ToListAsync(cancellationToken);
        var runningParts = parts.Where(x => x.StatusId == TestAttemptPartStatusIds.InProgress).ToList();
        if (runningParts.Count != 1)
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);
        var part = runningParts[0];
        if (part.StartedAt is null || part.DurationMinutes is not > 0 || part.EffectiveDeadlineAt is null
            || part.LastQuestionNo is not > 0)
            return Failure(ErrorsCodes.CandidateExamCurrentStateInvalid);

        var matchingQuestions = await unitOfWork.Context.Set<TestAttemptQuestion>()
            .AsNoTracking()
            .Where(x => x.TestAttemptPartId == part.Id && x.OrderNo == part.LastQuestionNo.Value)
            .Select(x => new CandidateExamCurrentQuestionData(x.Id, x.OrderNo,
                isArabic
                    ? x.QuestionRevision!.QuestionTextAr
                    : x.QuestionRevision!.QuestionTextEn ?? x.QuestionRevision.QuestionTextAr,
                x.QuestionRevision!.ResourceId, x.SelectedQuestionRevisionOptionId, x.SavedAt,
                x.QuestionRevision.QuestionType.Id, x.QuestionRevision.QuestionType.BackendName,
                isArabic ? x.QuestionRevision.QuestionType.NameAr : x.QuestionRevision.QuestionType.NameEn,
                isArabic
                    ? x.QuestionRevision.QuestionType.DescriptionAr
                    : x.QuestionRevision.QuestionType.DescriptionEn,
                x.QuestionRevision.Options.OrderBy(o => o.DisplayOrder)
                    .Select(o => new CandidateExamCurrentQuestionOptionData(o.Id,
                        isArabic ? o.OptionTextAr : o.OptionTextEn ?? o.OptionTextAr, o.DisplayOrder, o.ResourceId))
                    .ToList()))
            .Take(2)
            .ToListAsync(cancellationToken);
        if (matchingQuestions.Count != 1)
            return Failure(ErrorsCodes.CandidateExamCurrentQuestionInvalid);
        var question = matchingQuestions[0];

        var totalQuestions = part.TotalQuestions;
        if (question.OrderNo > totalQuestions)
            return Failure(ErrorsCodes.CandidateExamCurrentQuestionInvalid);

        var resourceIds = question.Options.Where(x => x.ResourceId.HasValue)
            .Select(x => x.ResourceId!.Value)
            .Append(question.ResourceId ?? Guid.Empty)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();
        var resources = resourceIds.Count == 0
            ? new Dictionary<Guid, CandidateExamCurrentQuestionResourceData>()
            : await unitOfWork.Context.Set<Resource>()
                .AsNoTracking()
                .Where(x => resourceIds.Contains(x.Id))
                .Select(x => new CandidateExamCurrentQuestionResourceData(x.Id, x.Name, x.Url))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

        FileRefDto? Resource(Guid? id) => id.HasValue && resources.TryGetValue(id.Value, out var value)
            ? new FileRefDto
            {
                ResourceId = value.Id, FileName = value.Name, Url = mediaUrlResolver.ResolveAbsolute(value.Url)
            }
            : null;

        var serverTimeUtc = timeProvider.GetUtcNow().UtcDateTime;

        var startedAtUtc = DateTime.SpecifyKind(part.StartedAt.Value, DateTimeKind.Utc);

        var effectiveDeadlineAtUtc = DateTime.SpecifyKind(part.EffectiveDeadlineAt.Value, DateTimeKind.Utc);

        var remainingSeconds = Math.Max(0L, (long)Math.Ceiling((effectiveDeadlineAtUtc - serverTimeUtc).TotalSeconds));
        var options = question.Options.Select(x => new CandidateExamQuestionOptionDto(
                x.Id, x.Text ?? string.Empty, x.DisplayOrder, Resource(x.ResourceId)))
            .ToList();

        return Result.Ok(new CandidateExamCurrentQuestionResponse(attempt.Id, part.Id, attempt.ExamId, part.PartNo,
            part.Title, part.PartNo, parts.Count, part.DurationMinutes.Value, startedAtUtc, effectiveDeadlineAtUtc,
            serverTimeUtc, remainingSeconds, remainingSeconds == 0, question.OrderNo, totalQuestions,
            attempt.AllowPreviousQuestion, attempt.AllowPreviousQuestion && question.OrderNo > 1,
            question.OrderNo < totalQuestions, question.Id,
            new DropdownOptions
            {
                Id = question.QuestionTypeId,
                BackendName = question.QuestionTypeBackendName,
                Name = question.QuestionTypeName,
                Description = question.QuestionTypeDescription
            }, question.Text ?? string.Empty, Resource(question.ResourceId), question.SelectedOptionId,
            question.SavedAt, options, attempt.JobTitle));
    }

    private static IResult<CandidateExamCurrentQuestionResponse> Failure(string code, int statusCode = 409) =>
        Result.Fail<CandidateExamCurrentQuestionResponse>(new FluentResults.Error(code).WithMetadata("Code", code)
            .WithMetadata("StatusCode", statusCode));
}
