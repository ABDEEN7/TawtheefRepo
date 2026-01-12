namespace Application.Operation.Features.Employee.Kawader.DTOs;

public sealed record KawaderUploadErrorDto(int RowNumber, string Value, string Reason);

public sealed record KawaderUploadResultDto
{
    public int ImportedCount { get; init; }
    public int ProcessedRows { get; init; }
    public IReadOnlyList<KawaderUploadErrorDto> Errors { get; init; } = [];
    public bool Success => Errors.Count == 0;
}
