using Tawtheef.Domain.Entities.Exams;

namespace Application.Recruitment.Features.CandidateExams.Common;

internal static class CandidateExamAttemptQueryExtensions
{
    public static IQueryable<TestAttempt> OwnedByApplicant(
        this IQueryable<TestAttempt> query,
        Guid applicantId) =>
        query.Where(attempt =>
            attempt.TestSessionCandidate!.Invitation!.ApplicantId ==
            applicantId);
}
