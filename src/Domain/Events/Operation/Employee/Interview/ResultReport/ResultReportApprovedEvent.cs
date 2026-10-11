using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.ResultReport;

/// <summary>The final approver approved the result report.</summary>
public sealed record ResultReportApprovedEvent(InterviewResultReport Report, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
