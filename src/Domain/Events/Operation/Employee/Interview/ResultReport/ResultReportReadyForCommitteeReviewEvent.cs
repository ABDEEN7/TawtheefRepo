using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Interview;

namespace Tawtheef.Domain.Events.Operation.Employee.Interview.ResultReport;

/// <summary>A newly generated result report is waiting for the committee chair's review.</summary>
public sealed record ResultReportReadyForCommitteeReviewEvent(InterviewResultReport Report, DateTimeOffset OnDateOccurred)
    : BaseEvent(OnDateOccurred);
