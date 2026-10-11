namespace Tawtheef.Domain.Constants;

public static class CandidateExamAttemptAudit
{
    public const string Section = "CandidateExamAttempt";

    public const string OperationalIssueReported = "CandidateExamOperationalIssueReported";

    public const string Rescheduled = "CandidateExamRescheduled";
}

public static class TestSessionWorkflowAudit
{
    public const string Section = "TestSessionWorkflow";

    public const string Approved = "TestSessionApproved";

    public const string Rejected = "TestSessionRejected";

    public const string ReturnedForEdit = "TestSessionReturnedForEdit";
}

public static class ExamWorkflowAudit
{
    public const string Section = "ExamWorkflow";

    public const string Approved = "ExamApproved";

    public const string Rejected = "ExamRejected";

    public const string ReturnedForEdit = "ExamReturnedForEdit";
}

public static class ExamResultReportAudit
{
    public const string Section = "ExamResultReport";

    public const string Approved = "ExamResultReportApproved";

    public const string ReturnedForProcessing = "ExamResultReportReturnedForProcessing";

    public const string CandidateCalculatedScoreAccepted = "ExamResultCandidateCalculatedScoreAccepted";

    public const string CandidateResolvedNotCompleted = "ExamResultCandidateResolvedNotCompleted";

    public const string CandidateRescheduled = "ExamResultCandidateRescheduled";
}
