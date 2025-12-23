using Microsoft.AspNetCore.Http;

namespace Tawtheef.Application.Features.Recruitment.Profile.Validators;

internal static class FileValidationHelpers
{
    public static bool HasFile(IFormFile? file) => file is { Length: > 0 };
    public static bool HasExisting(string? fileName) => !string.IsNullOrWhiteSpace(fileName);
}
