namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamQuestionGenerationQuotaSlot(
    Guid CategoryId, List<CandidateExamQuestionGenerationMembershipData> Candidates);
