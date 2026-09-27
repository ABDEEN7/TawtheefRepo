namespace Application.Recruitment.Features.CandidateExams.Models;

public sealed record CandidateExamCalculatedScores(
    decimal SpecializedScore,
    decimal SpecializedQualificationThreshold,
    bool IsSpecializedQualified,
    decimal? EducationalScore,
    decimal? SkillsScore,
    decimal? FinalScore);
