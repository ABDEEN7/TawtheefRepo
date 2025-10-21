using System;
using System.Collections.Generic;

namespace Tawtheef.Application.Common.Exceptions;

public class UnauthorizedException(string message, IEnumerable<string> errors) : Exception(message)
{
    public IEnumerable<string> Errors { get; } = errors;
}