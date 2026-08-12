using System.Text.Json;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.Spanshot;

namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals;

internal static class PendingQualificationSnapshotParser
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static PendingQualificationSnapshot? Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize<PendingQualificationSnapshot>(json, JsonOptions);
    }
}
