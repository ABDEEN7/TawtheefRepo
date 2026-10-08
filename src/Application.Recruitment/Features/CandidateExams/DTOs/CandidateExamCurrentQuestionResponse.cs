using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;

namespace Application.Recruitment.Features.CandidateExams.DTOs;

public sealed record CandidateExamCurrentQuestionResponse(
    Guid TestAttemptId,
    Guid TestAttemptPartId,
    Guid ExamId,
    int PartNo,
    string PartTitle,
    int CurrentPartNumber,
    int TotalParts,
    int DurationMinutes,
    DateTimeOffset StartedAt,
    DateTimeOffset EffectiveDeadlineAt,
    DateTimeOffset ServerTimeUtc,
    long RemainingSeconds,
    bool IsTimeExpired,
    int CurrentQuestionNumber,
    int TotalQuestionsInPart,
    bool AllowPreviousQuestion,
    bool HasPrevious,
    bool HasNext,
    Guid TestAttemptQuestionId,
    DropdownOptions QuestionType,
    string QuestionText,
    FileRefDto? Resource,
    Guid? SelectedOptionId,
    DateTimeOffset? SavedAt,
    IReadOnlyList<CandidateExamQuestionOptionDto> Options,
    string JobTitle);
