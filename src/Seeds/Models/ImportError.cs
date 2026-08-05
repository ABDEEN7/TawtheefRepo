namespace Seeds.Models;

public sealed record ImportError(string FileName, int? RowNumber, string Message);
