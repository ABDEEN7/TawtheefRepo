namespace Tawtheef.Application.Common.Concurrency;

public static class ApplicationLockResources
{
    private const string Root = "Tawtheef";

    public static string CandidateExamAttempt(Guid attemptId) =>
        $"{Root}.CandidateExam.Attempt.{attemptId:D}";

    public static string CandidateExamTestSlot(Guid testSlotId) =>
        $"{Root}.CandidateExam.TestSlot.{testSlotId:D}";

    public static string CandidateExamTestSessionCandidate(Guid testSessionCandidateId) =>
        $"{Root}.CandidateExam.TestSessionCandidate.{testSessionCandidateId:D}";

    public static string CandidateExamResult(Guid examId) =>
        $"{Root}.CandidateExam.Result.{examId:D}";
}
