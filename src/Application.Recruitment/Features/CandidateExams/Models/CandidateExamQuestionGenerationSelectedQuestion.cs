namespace Application.Recruitment.Features.CandidateExams.Models;

internal sealed record CandidateExamQuestionGenerationSelectedQuestion(
    Guid CategoryId, CandidateExamQuestionGenerationMembershipData Membership);
