using FluentResults;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace Tawtheef.Infrastructure.Services.StorageServices;

public sealed class LocalStorageService : IFileStorageService
{
    private readonly string _rootFull;
    private readonly string _publicBaseUrl;
    private readonly IAppLogger _logger;
    private static readonly FileExtensionContentTypeProvider Ct = new();
    public LocalStorageService(IOptions<StorageSettings> storageSettings, IAppLogger logger)
    {
        var root = storageSettings.Value.RootPath ?? throw new InvalidOperationException("Storage:RootPath missing");
        _publicBaseUrl = storageSettings.Value.PublicBaseUrl;

        _rootFull = Path.GetFullPath(root);
        Directory.CreateDirectory(_rootFull);
        _logger = logger.ForContext(typeof(LocalStorageService));
    }

    public async Task<IResult<FileSaved>> SaveAsync(Stream stream, string blobKey, CancellationToken ct = default)
    {
        string? temporaryPath = null;
        try
        {
            ct.ThrowIfCancellationRequested();

            var mapRes = MapPath(blobKey);
            if (!mapRes.IsSuccess) return Result.Fail<FileSaved>(mapRes.Errors);

            var full = mapRes.Value!;
            var dir = Path.GetDirectoryName(full);
            if (string.IsNullOrWhiteSpace(dir))
                return Result.Fail<FileSaved>(ErrorsCodes.InvalidBlobKey);

            Directory.CreateDirectory(dir);

            // atomic write: .tmp then move
            var tmp = full + "." + Guid.NewGuid().ToString("N") + ".tmp";
            await using (var fs = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None, 64 * 1024, useAsync: true))
            {
                temporaryPath = tmp;
                await stream.CopyToAsync(fs, ct);
            }

            if (File.Exists(full)) File.Delete(full);
            File.Move(tmp, full);
            temporaryPath = null;

            var size = new FileInfo(full).Length;
            return Result.Ok(new FileSaved(blobKey, (ulong)size));
        }
        catch (OperationCanceledException oce)
        {
            _logger.Error(oce, "SaveAsync cancelled for {BlobKey}", blobKey);
            return Result.Fail<FileSaved>(ErrorsCodes.Cancelled);
        }
        catch (UnauthorizedAccessException uae)
        {
            _logger.Error(uae, "SaveAsync access denied for {BlobKey}", blobKey);
            return Result.Fail<FileSaved>(ErrorsCodes.AccessDenied);
        }
        catch (IOException ioe)
        {
            _logger.Error(ioe, "SaveAsync IO error for {BlobKey}", blobKey);
            return Result.Fail<FileSaved>(ErrorsCodes.IoError);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "SaveAsync error for {BlobKey}", blobKey);
            return Result.Fail<FileSaved>(ErrorsCodes.IoError);
        }
        finally
        {
            if (temporaryPath != null)
            {
                try { File.Delete(temporaryPath); }
                catch (Exception ex) { _logger.Error(ex, "Failed to remove temporary upload for {BlobKey}", blobKey); }
            }
        }
    }

    public Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            var mapRes = MapPath(blobKey, true);
            if (!mapRes.IsSuccess) return Task.FromResult(Result.Fail<bool>(mapRes.Errors));

            var full = mapRes.Value!;
            if (!File.Exists(full))
                return Task.FromResult(Result.Ok(false));

            File.Delete(full);
            return Task.FromResult(Result.Ok(true));
        }
        catch (OperationCanceledException oce)
        {
            _logger.Error(oce, "DeleteAsync cancelled for {BlobKey}", blobKey);
            return Task.FromResult(Result.Fail<bool>(ErrorsCodes.Cancelled));
        }
        catch (UnauthorizedAccessException uae)
        {
            _logger.Error(uae, "DeleteAsync access denied for {BlobKey}", blobKey);
            return Task.FromResult(Result.Fail<bool>(ErrorsCodes.AccessDenied));
        }
        catch (IOException ioe)
        {
            _logger.Error(ioe, "DeleteAsync IO error for {BlobKey}", blobKey);
            return Task.FromResult(Result.Fail<bool>(ErrorsCodes.IoError));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "DeleteAsync error for {BlobKey}", blobKey);
            return Task.FromResult(Result.Fail<bool>(ErrorsCodes.IoError));
        }
    }

    public IResult<string> ToPublicUrl(string blobKey)
    {
        if (string.IsNullOrWhiteSpace(_publicBaseUrl))
            return Result.Fail<string>(ErrorsCodes.ConfigMissing);

        if (!StoragePathRules.TryNormalizeKey(blobKey, out var key))
            return Result.Fail<string>(ErrorsCodes.InvalidBlobKey);

        if (!key.StartsWith("public/", StringComparison.Ordinal))
            return Result.Fail<string>(ErrorsCodes.NotPublicResource);

        var mapped = MapPath(key);
        if (mapped.IsFailed) return Result.Fail<string>(mapped.Errors);

        var url = _publicBaseUrl.TrimEnd('/') + "/" + Uri.EscapeDataString(key["public/".Length..]);
        return Result.Ok(url);
    }

    public IResult<string> MapPath(string blobKey, bool isReadOperation = false)
    {
        try
        {
            if (!StoragePathRules.TryNormalizeKey(blobKey, out var key))
                return Result.Fail<string>(ErrorsCodes.InvalidBlobKey);

            var combined = Path.GetFullPath(Path.Combine(_rootFull, key.Replace('/', Path.DirectorySeparatorChar)));
            var scope = key[..key.IndexOf('/')];
            var scopeRoot = Path.GetFullPath(Path.Combine(_rootFull, scope));
            var scopePrefix = Path.EndsInDirectorySeparator(scopeRoot)
                ? scopeRoot : scopeRoot + Path.DirectorySeparatorChar;
            var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            // A separator-terminated prefix prevents sibling-prefix matches. Each
            // scope has its own boundary: public keys can never resolve to private.
            if (!combined.StartsWith(scopePrefix, comparison) || HasLinkedComponent(key))
                return Result.Fail<string>(ErrorsCodes.InvalidBlobKey);

            // If not read operation → just return path
            if (!isReadOperation)
                return Result.Ok(combined);

            // Read operation → ensure file exists
            if (!File.Exists(combined))
                return Result.Fail<string>(ErrorsCodes.FileNotFound);

            return Result.Ok(combined);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "MapPath error for {BlobKey}", blobKey);
            return Result.Fail<string>(ErrorsCodes.InvalidBlobKey);
        }
    }
    
    public Task<bool> ExistsAsync(string path, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var mapped = MapPath(path, isReadOperation: true);
        return Task.FromResult(mapped.IsSuccess);
    }

    public Task<StoredFileStream?> OpenReadAsync(string path, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var mapped = MapPath(path, isReadOperation: true);
        if (mapped.IsFailed) return Task.FromResult<StoredFileStream?>(null);
        var full = mapped.Value;

        var stream = File.OpenRead(full);
        if (!Ct.TryGetContentType(full, out var contentType))
            contentType = "application/octet-stream";

        var fi = new FileInfo(full);
        var etag = $"\"{fi.LastWriteTimeUtc.Ticks:x}-{fi.Length:x}\""; // cheap ETag
        var lastModified = new DateTimeOffset(fi.LastWriteTimeUtc);

        return Task.FromResult<StoredFileStream?>(new(
            Stream: stream,
            ContentType: contentType,
            ETag: etag,
            LastModified: lastModified,
            Length: fi.Length
        ));
    }

    private bool HasLinkedComponent(string key)
    {
        var current = _rootFull;
        foreach (var segment in key.Split('/'))
        {
            current = Path.Combine(current, segment);
            try
            {
                // Includes file/directory symlinks and Windows junctions. Reject
                // rather than following a link across the scope's directory boundary.
                if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) return true;
            }
            catch (FileNotFoundException) { return false; }
            catch (DirectoryNotFoundException) { return false; }
        }
        return false;
    }

    public Task<IResult<string>> GetSignedReadUrlAsync(string blobKey, TimeSpan ttl, string? downloadName = null, CancellationToken ct = default)
        => Task.FromResult<IResult<string>>(Result.Fail<string>(ErrorsCodes.NotSupported));
}
