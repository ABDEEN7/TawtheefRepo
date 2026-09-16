namespace Tawtheef.Domain.Constants;

public static class InterviewScheduleAuditActions
{
    public const string Created = "Created";
    public const string Resubmitted = "Resubmitted";
    public const string Approved = "Approved";
    public const string Returned = "Returned";
    public const string Cancelled = "Cancelled";
    public const string ReadyForExecution = "ReadyForExecution";
    public const string ExecutionStarted = "ExecutionStarted";
    public const string Closed = "Closed";
}

public static class InterviewAppointmentAuditActions
{
    public const string AttendanceRecorded = "AttendanceRecorded";
    public const string InterviewStarted = "InterviewStarted";
    public const string UnderEvaluation = "UnderEvaluation";
    public const string EvaluationCompleted = "EvaluationCompleted";
    public const string Closed = "Closed";
    public const string Cancelled = "Cancelled";
    public const string Rescheduled = "Rescheduled";
}
