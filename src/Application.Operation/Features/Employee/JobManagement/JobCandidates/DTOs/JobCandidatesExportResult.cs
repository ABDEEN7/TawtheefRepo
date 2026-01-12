namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;

public sealed class JobCandidatesExportResult
{
    public required byte[] Content { get; init; }
    public string ContentType { get; init; } = "text/csv";
    public required string FileName { get; init; }
}
