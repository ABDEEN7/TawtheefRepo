namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;

public sealed class JobCandidatesExportResult
{
    public required byte[] Content { get; init; }
    public string ContentType { get; init; }
        = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public required string FileName { get; init; }
}
