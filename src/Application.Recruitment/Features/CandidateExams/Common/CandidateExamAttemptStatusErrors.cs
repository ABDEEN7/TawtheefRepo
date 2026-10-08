using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Recruitment.Features.CandidateExams.Common;

internal static class CandidateExamAttemptStatusErrors
{
    public static string Resolve(Guid statusId) =>
        statusId == TestAttemptStatusIds.NotStarted ? ErrorsCodes.CandidateExamAttemptNotStarted :
        statusId == TestAttemptStatusIds.Interrupted ? ErrorsCodes.CandidateExamAttemptInterrupted :
        statusId == TestAttemptStatusIds.Completed ? ErrorsCodes.CandidateExamAttemptCompleted :
        statusId == TestAttemptStatusIds.TimedOut ? ErrorsCodes.CandidateExamAttemptTimedOut :
        statusId == TestAttemptStatusIds.Cancelled ? ErrorsCodes.CandidateExamAttemptCancelled :
        ErrorsCodes.CandidateExamCurrentStateInvalid;
}
