using FluentResults;
using Microsoft.AspNetCore.Http;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Admin.HomeContent.SuccessStories.Handlers.Commands;

internal static class HomeSuccessStoryImageValidator
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp",
        ".bmp"
    };

    public static Result Validate(IFormFile file)
    {
        var fileName = Path.GetFileName(file.FileName).Trim();
        var extension = Path.GetExtension(fileName);

        if (string.IsNullOrWhiteSpace(fileName) ||
            string.IsNullOrWhiteSpace(extension) ||
            !AllowedExtensions.Contains(extension))
        {
            return Result.Fail(ErrorsCodes.InvalidFileType);
        }

        using var stream = file.OpenReadStream();
        if (!TryReadAllBytes(stream, out var bytes) || bytes.Length == 0)
        {
            return Result.Fail(ErrorsCodes.InvalidFileType);
        }

        if (IsJpeg(bytes) || IsBmp(bytes))
        {
            return Result.Ok();
        }

        if (IsPng(bytes))
        {
            return HasPngAnimationChunk(bytes)
                ? Result.Fail(ErrorsCodes.InvalidFileType)
                : Result.Ok();
        }

        if (IsWebP(bytes))
        {
            return HasWebPAnimationChunk(bytes)
                ? Result.Fail(ErrorsCodes.InvalidFileType)
                : Result.Ok();
        }

        return Result.Fail(ErrorsCodes.InvalidFileType);
    }

    private static bool IsJpeg(ReadOnlySpan<byte> bytes)
        => HasPrefix(bytes, [0xFF, 0xD8, 0xFF]);

    private static bool IsBmp(ReadOnlySpan<byte> bytes)
        => HasPrefix(bytes, [0x42, 0x4D]);

    private static bool IsPng(ReadOnlySpan<byte> bytes)
        => HasPrefix(bytes, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]);

    private static bool IsWebP(ReadOnlySpan<byte> bytes)
        => bytes.Length >= 12 &&
           bytes[..4].SequenceEqual("RIFF"u8) &&
           bytes.Slice(8, 4).SequenceEqual("WEBP"u8);

    private static bool HasPrefix(ReadOnlySpan<byte> bytes, ReadOnlySpan<byte> expected)
        => bytes.Length >= expected.Length && bytes[..expected.Length].SequenceEqual(expected);

    private static bool HasPngAnimationChunk(ReadOnlySpan<byte> bytes)
    {
        const int pngSignatureLength = 8;
        var offset = pngSignatureLength;

        while (offset + 8 <= bytes.Length)
        {
            var chunkLength = ReadBigEndianUInt32(bytes, offset);
            if (chunkLength < 0)
                return false;

            var chunkTypeOffset = offset + 4;
            if (chunkTypeOffset + 4 > bytes.Length)
                return false;

            if (bytes.Slice(chunkTypeOffset, 4).SequenceEqual("acTL"u8))
                return true;

            var next = offset + 12L + chunkLength;
            if (next > bytes.Length)
                return false;

            offset = (int)next;
        }

        return false;
    }

    private static bool HasWebPAnimationChunk(ReadOnlySpan<byte> bytes)
    {
        var offset = 12;

        while (offset + 8 <= bytes.Length)
        {
            var chunkType = bytes.Slice(offset, 4);
            var chunkLength = ReadLittleEndianUInt32(bytes, offset + 4);
            if (chunkLength < 0)
                return false;

            if (chunkType.SequenceEqual("ANIM"u8))
                return true;

            if (chunkType.SequenceEqual("VP8X"u8))
            {
                if (offset + 9 > bytes.Length)
                    return false;

                const byte animationFlag = 0b0000_0010;
                if ((bytes[offset + 8] & animationFlag) != 0)
                    return true;
            }

            var paddedChunkLength = chunkLength + (chunkLength % 2);
            var next = offset + 8L + paddedChunkLength;
            if (next > bytes.Length)
                return false;

            offset = (int)next;
        }

        return false;
    }

    private static bool TryReadAllBytes(Stream stream, out byte[] bytes)
    {
        bytes = [];

        try
        {
            if (stream.CanSeek)
                stream.Position = 0;

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
            return true;
        }
        finally
        {
            if (stream.CanSeek)
                stream.Position = 0;
        }
    }

    private static int ReadBigEndianUInt32(ReadOnlySpan<byte> bytes, int offset)
    {
        if (offset + 4 > bytes.Length)
            return -1;

        return (bytes[offset] << 24) |
               (bytes[offset + 1] << 16) |
               (bytes[offset + 2] << 8) |
               bytes[offset + 3];
    }

    private static int ReadLittleEndianUInt32(ReadOnlySpan<byte> bytes, int offset)
    {
        if (offset + 4 > bytes.Length)
            return -1;

        return bytes[offset] |
               (bytes[offset + 1] << 8) |
               (bytes[offset + 2] << 16) |
               (bytes[offset + 3] << 24);
    }
}
