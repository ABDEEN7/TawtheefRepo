using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using Serilog;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Configurations.Settings;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Infrastructure.Services.StorageServices;

public sealed class LocalStorageService : IFileStorageService
{
    private readonly string _rootFull;
    private readonly string _publicBaseUrl;
    private readonly ILogger _logger;

    public LocalStorageService(IOptions<StorageSettings> storageSettings, ILogger logger)
    {
        var root = storageSettings.Value.RootPath ?? throw new InvalidOperationException("Storage:RootPath missing");
        _publicBaseUrl = storageSettings.Value.PublicBaseUrl;

        _rootFull = Path.GetFullPath(root);
        Directory.CreateDirectory(_rootFull);
        _logger = logger.ForContext<LocalStorageService>();
    }

    public async Task<Result<FileSaved>> SaveAsync(Stream stream, string blobKey, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            var mapRes = MapPath(blobKey);
            if (!mapRes.IsSuccess) return Result.Failure<FileSaved>(mapRes.Error);

            var full = mapRes.Value!;
            var dir = Path.GetDirectoryName(full);
            if (string.IsNullOrWhiteSpace(dir))
                return Result.Failure<FileSaved>(ErrorsCodes.InvalidBlobKey);

            Directory.CreateDirectory(dir);

            // atomic write: .tmp then move
            var tmp = full + ".tmp";
            await using (var fs = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None, 64 * 1024, useAsync: true))
            {
                await stream.CopyToAsync(fs, ct);
            }

            if (File.Exists(full)) File.Delete(full);
            File.Move(tmp, full);

            var size = new FileInfo(full).Length;
            return Result.Success(new FileSaved(blobKey, size));
        }
        catch (OperationCanceledException oce)
        {
            _logger.Error(oce, "SaveAsync cancelled for {BlobKey}", blobKey);
            return Result.Failure<FileSaved>(ErrorsCodes.Cancelled);
        }
        catch (UnauthorizedAccessException uae)
        {
            _logger.Error(uae, "SaveAsync access denied for {BlobKey}", blobKey);
            return Result.Failure<FileSaved>(ErrorsCodes.AccessDenied);
        }
        catch (IOException ioe)
        {
            _logger.Error(ioe, "SaveAsync IO error for {BlobKey}", blobKey);
            return Result.Failure<FileSaved>(ErrorsCodes.IoError);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "SaveAsync error for {BlobKey}", blobKey);
            return Result.Failure<FileSaved>(ErrorsCodes.IoError);
        }
    }

    public Task<Result<bool>> DeleteAsync(string blobKey, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            var mapRes = MapPath(blobKey);
            if (!mapRes.IsSuccess) return Task.FromResult(Result.Failure<bool>(mapRes.Error));

            var full = mapRes.Value!;
            if (!File.Exists(full))
                return Task.FromResult(Result.Success(false));

            File.Delete(full);
            return Task.FromResult(Result.Success(true));
        }
        catch (OperationCanceledException oce)
        {
            _logger.Error(oce, "DeleteAsync cancelled for {BlobKey}", blobKey);
            return Task.FromResult(Result.Failure<bool>(ErrorsCodes.Cancelled));
        }
        catch (UnauthorizedAccessException uae)
        {
            _logger.Error(uae, "DeleteAsync access denied for {BlobKey}", blobKey);
            return Task.FromResult(Result.Failure<bool>(ErrorsCodes.AccessDenied));
        }
        catch (IOException ioe)
        {
            _logger.Error(ioe, "DeleteAsync IO error for {BlobKey}", blobKey);
            return Task.FromResult(Result.Failure<bool>(ErrorsCodes.IoError));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "DeleteAsync error for {BlobKey}", blobKey);
            return Task.FromResult(Result.Failure<bool>(ErrorsCodes.IoError));
        }
    }

    public Task<Result<int>> DeletePrefixAsync(string prefix, CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            // Map *directory* for prefix
            var mapped = MapPath(prefix);
            if (!mapped.IsSuccess) return Task.FromResult(Result.Failure<int>(mapped.Error));

            var fullPrefix = mapped.Value!;
            // Ensure it's a directory path
            if (!Directory.Exists(fullPrefix))
                return Task.FromResult(Result.Success(0));

            var deleted = 0;

            foreach (var file in Directory.EnumerateFiles(fullPrefix, "*", SearchOption.AllDirectories))
            {
                ct.ThrowIfCancellationRequested();
                File.Delete(file);
                deleted++;
            }

            // cleanup dirs (best-effort)
            foreach (var dir in Directory.EnumerateDirectories(fullPrefix, "*", SearchOption.AllDirectories)
                         .OrderByDescending(d => d.Length))
            {
                TryDeleteDir(dir);
            }
            TryDeleteDir(fullPrefix);

            return Task.FromResult(Result.Success(deleted));

            static void TryDeleteDir(string d)
            {
                try { Directory.Delete(d); } catch { /* ignore */ }
            }
        }
        catch (OperationCanceledException oce)
        {
            _logger.Error(oce, "DeletePrefixAsync cancelled for {Prefix}", prefix);
            return Task.FromResult(Result.Failure<int>(ErrorsCodes.Cancelled));
        }
        catch (UnauthorizedAccessException uae)
        {
            _logger.Error(uae, "DeletePrefixAsync access denied for {Prefix}", prefix);
            return Task.FromResult(Result.Failure<int>(ErrorsCodes.AccessDenied));
        }
        catch (IOException ioe)
        {
            _logger.Error(ioe, "DeletePrefixAsync IO error for {Prefix}", prefix);
            return Task.FromResult(Result.Failure<int>(ErrorsCodes.IoError));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "DeletePrefixAsync error for {Prefix}", prefix);
            return Task.FromResult(Result.Failure<int>(ErrorsCodes.IoError));
        }
    }

    public Result<string> ToPublicUrl(string blobKey)
    {
        if (string.IsNullOrWhiteSpace(_publicBaseUrl))
            return Result.Failure<string>(ErrorsCodes.ConfigMissing);

        if (!blobKey.StartsWith("public/", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<string>(ErrorsCodes.NotPublicResource);

        var url = _publicBaseUrl.TrimEnd('/') + "/" + blobKey["public/".Length..].Replace("\\", "/");
        return Result.Success(url);
    }

    public Result<string> MapPath(string blobKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(blobKey))
                return Result.Failure<string>(ErrorsCodes.InvalidBlobKey);

            // Normalize and ensure under root (block path traversal)
            var combined = Path.GetFullPath(Path.Combine(_rootFull, blobKey.Replace('/', Path.DirectorySeparatorChar)));

            // allow the case where combined == _rootFull (prefix pointing to root subdir)
            return !combined.StartsWith(_rootFull, StringComparison.Ordinal) ? 
                Result.Failure<string>(ErrorsCodes.InvalidBlobKey) :
                Result.Success(combined);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "MapPath error for {BlobKey}", blobKey);
            return Result.Failure<string>(ErrorsCodes.InvalidBlobKey);
        }
    }
}
