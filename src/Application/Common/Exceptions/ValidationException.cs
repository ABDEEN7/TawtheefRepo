namespace Tawtheef.Application.Common.Exceptions;

public class ValidationException(List<ValidationError> errors) : Exception
{
    public List<ValidationError> Errors { get; } = errors;
}

public class ValidationError
{
    public string Code { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? Field { get; set; }
}
