namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummary.Contracts;

public interface IJobInvitationSummaryFilter
{
    string? Search { get; }
    Guid? JobCategoryId { get; }
    Guid? DepartmentId { get; }
    Guid? JobStatusId { get; }
    int? Year { get; }
}
