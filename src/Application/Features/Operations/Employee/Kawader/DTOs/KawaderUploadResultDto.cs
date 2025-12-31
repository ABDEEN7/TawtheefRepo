namespace Tawtheef.Application.Features.Operations.Employee.Kawader.DTOs;

public sealed record KawaderUploadErrorDto(int RowNumber, string Value, string Reason);

public sealed record KawaderUploadResultDto
{
    public int ImportedCount { get; init; }
    public int ProcessedRows { get; init; }
    public IReadOnlyList<KawaderUploadErrorDto> Errors { get; init; } = Array.Empty<KawaderUploadErrorDto>();
    public bool Success => Errors.Count == 0;
}
