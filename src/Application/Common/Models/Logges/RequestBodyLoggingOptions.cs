using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Application.Common.Models.Logges;

public sealed class RequestBodyLoggingOptions
{
    public const string SectionName = "Logging:RequestBody";

    public bool Enabled { get; init; } = false;
    public RequestBodyLoggingMode Mode { get; init; } = RequestBodyLoggingMode.DevelopmentOnly;

    [Range(0, int.MaxValue)]
    public int MaxLoggedBytes { get; init; } = 4 * 1024;

    [Range(0, int.MaxValue)]
    public int BufferThreshold { get; init; } = 64 * 1024;

    [Range(0, int.MaxValue)]
    public int BufferLimit { get; init; } = 4 * 1024 * 1024;

    public string[] AllowPaths { get; init; } = [];
    public string[] DenyPaths { get; init; } = [];

    public string[] SensitiveKeys { get; init; } =
    [
        "password","token","otp","secret","authorization"
    ];
}
