namespace Application.Operation.Features.Employee.Dashboard.DTOs.Export;

public sealed class DashboardExportResult
{
    public required byte[] Content { get; init; }
    public required string FileName { get; init; }
    public string ContentType { get; init; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
