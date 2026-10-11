namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamQuestionGenerationCategoryData(
    Guid Id,
    Guid ExamPartId,
    Guid QuestionBankTypeId,
    Guid QuestionBankVersionId,
    int QuestionCount,
    decimal WeightPercent,
    int EasyQuestionCount,
    int MediumQuestionCount,
    int HardQuestionCount,
    DateTime? VersionApprovedAt,
    Guid VersionQuestionBankTypeId);
