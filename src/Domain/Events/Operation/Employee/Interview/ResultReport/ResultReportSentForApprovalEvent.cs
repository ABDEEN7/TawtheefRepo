using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.ResultReport;

/// <summary>The committee chair sent the result report on to final approval.</summary>
public sealed record ResultReportSentForApprovalEvent(InterviewResultReport Report, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
