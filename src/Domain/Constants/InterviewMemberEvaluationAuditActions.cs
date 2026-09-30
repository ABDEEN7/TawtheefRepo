using System;
using System.Collections.Generic;
using System.Text;

namespace Tawtheef.Domain.Constants;

public static class InterviewMemberEvaluationAuditActions
{
    public const string DraftSaved = "DraftSaved";
    public const string Submitted = "Submitted";

    // this for future use, if we want to allow committee members to reopen their submitted evaluation and edit it again.
    public const string reopened = "Reopened"; 
}
public static class InterviewOperationalIssueAuditActions
{
    public const string Created = "Created";
    public const string Resolved = "Resolved";
    public const string Waived = "Waived";
    public const string BlockingUpdated = "BlockingUpdated";
}

public static class InterviewResultReportAuditActions
{
    public const string Generated = "Generated";
    public const string GenerationSkippedUnsupportedMethod = "GenerationSkippedUnsupportedMethod";
    public const string Approved = "Approved";
    public const string Returned = "Returned";
    // Soft-deleted because its candidate set changed (an appointment was rescheduled) before approval. // we need to discuss in BRD
    public const string Discarded = "Discarded";
    // Committee Head Review - the chair saved recommendations, or sent the report on to final approval.
    public const string CommitteeReviewSaved = "CommitteeReviewSaved";
    public const string SentForApproval = "SentForApproval";
}
