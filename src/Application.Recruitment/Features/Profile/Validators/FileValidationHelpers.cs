using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.Validators;

internal static class FileValidationHelpers
{
    public static bool HasFile(IFormFile? file) => file is { Length: > 0 };
    public static bool HasExisting(string? fileName) => !string.IsNullOrWhiteSpace(fileName);
}
