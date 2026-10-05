namespace Tawtheef.Application.Common.Concurrency;

public static class ApplicationLockResources
{
    private const string Root = "Tawtheef";

    public static string CandidateExamAttempt(Guid attemptId) =>
        $"{Root}.CandidateExam.Attempt.{attemptId:D}";

    public static string CandidateExamTestSlot(Guid? testSlotId) =>
        $"{Root}.CandidateExam.TestSlot.{testSlotId:D}";

    public static string CandidateExamTestSessionCandidate(Guid testSessionCandidateId) =>
        $"{Root}.CandidateExam.TestSessionCandidate.{testSessionCandidateId:D}";

    public static string CandidateExamResult(Guid examId) =>
        $"{Root}.CandidateExam.Result.{examId:D}";

    public static string TestSessionRoomDate(Guid roomId, DateOnly slotDate) =>
        $"{Root}.TestSession.Room.{roomId:D}.{slotDate:yyyyMMdd}";

    public static string ExamResultReportWorkflow(Guid reportId) =>
        $"{Root}.ExamResultReport.Approve.{reportId:D}";

    public static string TestSessionWorkflow(Guid testSessionId) =>
        $"{Root}.TestSession.Workflow.{testSessionId:D}";

    public static string TestSessionNumbering(int year) =>
        $"{Root}.TestSession.Numbering.{year}";

    public static string TestSessionCandidate(Guid jobId, Guid invitationId) =>
        $"{Root}.TestSession.Candidate.{jobId:D}.{invitationId:D}";

    public static string ExamWorkflow(Guid examId) =>
        $"{Root}.Exam.Workflow.{examId:D}";

    public static string ExamNumbering() =>
        $"{Root}.Exam.Numbering";
}
