using Microsoft.AspNetCore.Http;

namespace Application.Recruitment.Features.Profile.Validators;

internal static class FileValidationHelpers
{
    /// <summary>Allowed MIME types for profile file uploads.</summary>
    private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "image/png",
        "image/jpeg"
    };

    /// <summary>Allowed file extensions for profile file uploads (lowercase, with dot).</summary>
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".png",
        ".jpg",
        ".jpeg"
    };

    public static bool HasFile(IFormFile? file) => file is { Length: > 0 };
    public static bool HasExisting(string? fileName) => !string.IsNullOrWhiteSpace(fileName);

    /// <summary>
    /// Returns true if the file has an allowed MIME type and extension (PDF, PNG, JPG, JPEG).
    /// </summary>
    public static bool IsAllowedFileType(IFormFile? file)
    {
        if (file is null or { Length: 0 }) return true; // no file to validate

        // Check MIME type
        if (!string.IsNullOrWhiteSpace(file.ContentType) &&
            !AllowedMimeTypes.Contains(file.ContentType))
            return false;

        // Check extension
        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext))
            return false;

        return AllowedExtensions.Contains(ext);
    }

    /// <summary>
    /// Returns true if ALL files in the list have allowed types.
    /// </summary>
    public static bool AreAllAllowedFileTypes(IEnumerable<IFormFile?>? files)
    {
        if (files is null) return true;
        return files.All(IsAllowedFileType);
    }
}
