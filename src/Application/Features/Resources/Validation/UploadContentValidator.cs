using System.IO.Compression;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Resources.Validation;

internal static class UploadContentValidator
{
    private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

    private static readonly UploadPolicy ProfileDocumentPolicy = new(
        [
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        ],
        new Dictionary<string, Func<Stream, bool>>(Comparer)
        {
            [".pdf"] = IsPdf,
            [".jpg"] = IsJpeg,
            [".jpeg"] = IsJpeg,
            [".png"] = IsPng,
            [".webp"] = IsWebP
        });

    private static readonly UploadPolicy ProfileCvPolicy = new(
        [
            ".pdf",
            ".doc",
            ".docx",
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        ],
        new Dictionary<string, Func<Stream, bool>>(Comparer)
        {
            [".pdf"] = IsPdf,
            [".doc"] = IsLegacyWordDocument,
            [".docx"] = IsWordOpenXmlDocument,
            [".jpg"] = IsJpeg,
            [".jpeg"] = IsJpeg,
            [".png"] = IsPng,
            [".webp"] = IsWebP
        });

    private static readonly UploadPolicy InvitationExceptionProofPolicy = new(
        new HashSet<string>(Comparer) { ".pdf" },
        new Dictionary<string, Func<Stream, bool>>(Comparer)
        {
            [".pdf"] = IsPdf
        },
        new HashSet<string>(Comparer) { "application/pdf" },
        ProfileLimits.MaxExperienceFileSizeBytes,
        ErrorsCodes.ExceptionProofFileTooLarge);

    public static Result Validate(string blobPath, IFormFile? file)
    {
        if (file is null or { Length: 0 })
            return Result.Fail(ErrorsCodes.EmptyFile);

        if (!TryResolvePolicy(blobPath, out var policy))
            return Result.Ok();

        if (policy.MaxFileSizeBytes.HasValue && file.Length > policy.MaxFileSizeBytes.Value)
            return Result.Fail(policy.FileTooLargeErrorCode ?? ErrorsCodes.InvalidFileType);

        if (policy.AllowedContentTypes is not null &&
            !policy.AllowedContentTypes.Contains(file.ContentType))
        {
            return Result.Fail(ErrorsCodes.InvalidFileType);
        }

        var fileName = Path.GetFileName(file.FileName).Trim();
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(fileName) ||
            string.IsNullOrWhiteSpace(extension) ||
            !policy.AllowedExtensions.Contains(extension) ||
            !policy.SignatureValidators.TryGetValue(extension, out var signatureValidator))
        {
            return Result.Fail(ErrorsCodes.InvalidFileType);
        }

        using var stream = file.OpenReadStream();
        return signatureValidator(stream)
            ? Result.Ok()
            : Result.Fail(ErrorsCodes.InvalidFileType);
    }

    private static bool TryResolvePolicy(string blobPath, out UploadPolicy policy)
    {
        policy = default!;

        var segments = blobPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (IsInvitationExceptionProofPath(segments))
        {
            policy = InvitationExceptionProofPolicy;
            return true;
        }

        if (segments.Length < 7 ||
            !segments[1].Equals("recruitment", StringComparison.OrdinalIgnoreCase) ||
            !segments[4].Equals("profile", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var resolvedPolicy = segments[5] switch
        {
            "cv" or "resume" => ProfileCvPolicy,
            "additional" or
            "experience" or
            "training" or
            "education" or
            "achievement" or
            "national-id" or
            "birth-certificate" or
            "marriage-certificate" or
            "national-address" or
            "sponsor-card" or
            "national-card" or
            "residence-certificate" => ProfileDocumentPolicy,
            _ => null
        };

        if (resolvedPolicy is null)
            return false;

        policy = resolvedPolicy;
        return true;
    }

    private static bool IsInvitationExceptionProofPath(IReadOnlyList<string> segments) =>
        segments.Count == 6 &&
        segments[0].Equals("private", StringComparison.OrdinalIgnoreCase) &&
        segments[1].Equals("operation", StringComparison.OrdinalIgnoreCase) &&
        segments[2].Equals("exceptions", StringComparison.OrdinalIgnoreCase) &&
        Guid.TryParse(segments[3], out _) &&
        segments[4].Equals("proof", StringComparison.OrdinalIgnoreCase);

    private static bool IsPdf(Stream stream)
        => HasPrefix(stream, "%PDF-"u8);

    private static bool IsJpeg(Stream stream)
        => HasPrefix(stream, [0xFF, 0xD8, 0xFF]);

    private static bool IsPng(Stream stream)
        => HasPrefix(stream, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);

    private static bool IsWebP(Stream stream)
    {
        Span<byte> header = stackalloc byte[12];
        if (!TryReadHeader(stream, header))
            return false;

        return header[..4].SequenceEqual("RIFF"u8) && header[8..12].SequenceEqual("WEBP"u8);
    }

    private static bool IsLegacyWordDocument(Stream stream)
        => HasPrefix(stream, [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1]);

    private static bool IsWordOpenXmlDocument(Stream stream)
    {
        if (!HasZipSignature(stream))
            return false;

        Reset(stream);

        try
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);

            return archive.Entries.Any(entry => entry.FullName.Equals("[Content_Types].xml", StringComparison.OrdinalIgnoreCase)) &&
                   archive.Entries.Any(entry => entry.FullName.StartsWith("word/", StringComparison.OrdinalIgnoreCase));
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    private static bool HasZipSignature(Stream stream)
    {
        Span<byte> header = stackalloc byte[4];
        if (!TryReadHeader(stream, header))
            return false;

        return IsMatch(header, 0x50, 0x4B, 0x03, 0x04) ||
               IsMatch(header, 0x50, 0x4B, 0x05, 0x06) ||
               IsMatch(header, 0x50, 0x4B, 0x07, 0x08);
    }

    private static bool HasPrefix(Stream stream, ReadOnlySpan<byte> expected)
    {
        Span<byte> header = stackalloc byte[expected.Length];
        return TryReadHeader(stream, header) && header.SequenceEqual(expected);
    }

    private static bool TryReadHeader(Stream stream, Span<byte> buffer)
    {
        Reset(stream);

        var totalRead = 0;
        while (totalRead < buffer.Length)
        {
            var read = stream.Read(buffer[totalRead..]);
            if (read == 0)
                break;

            totalRead += read;
        }

        Reset(stream);
        return totalRead == buffer.Length;
    }

    private static void Reset(Stream stream)
    {
        if (stream.CanSeek)
            stream.Position = 0;
    }

    private static bool IsMatch(ReadOnlySpan<byte> header, byte b0, byte b1, byte b2, byte b3)
        => header[0] == b0 && header[1] == b1 && header[2] == b2 && header[3] == b3;

    private sealed record UploadPolicy(
        HashSet<string> AllowedExtensions,
        IReadOnlyDictionary<string, Func<Stream, bool>> SignatureValidators,
        HashSet<string>? AllowedContentTypes = null,
        long? MaxFileSizeBytes = null,
        string? FileTooLargeErrorCode = null);
}
