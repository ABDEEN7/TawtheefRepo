using System;

namespace Tawtheef.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base()
    {
    }
}