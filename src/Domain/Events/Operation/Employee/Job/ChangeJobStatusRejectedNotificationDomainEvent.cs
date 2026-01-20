using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.Operation.Employee.Job;
using JobEntity = Entities.Recruitment.Job;

public sealed record ChangeJobStatusRejectedNotificationDomainEvent(JobEntity Job, DateTimeOffset DateOccurred) 
    : BaseEvent(DateOccurred);
