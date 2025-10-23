using System;
using System.Collections.Generic;

namespace Tawtheef.Application.Common.Exceptions;

public class NotFoundException(string message, IEnumerable<string> errors) : Exception(message)
{
    public IEnumerable<string> Errors { get; } = errors;
}
