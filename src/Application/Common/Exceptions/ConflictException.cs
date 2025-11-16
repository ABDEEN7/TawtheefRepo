namespace Tawtheef.Application.Common.Exceptions;

public class ConflictException(string message, IEnumerable<string> errors) : Exception(message)
{
    public IEnumerable<string> Errors { get; } = errors;
}
