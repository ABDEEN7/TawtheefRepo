using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Queries;

namespace Tawtheef.Application.Features.Resources.Handlers.Queries;

public sealed class GetPublicFileHandler(IFileStorageService storage) : IRequestHandler<GetPublicFileQuery, IActionResult>
{
    public async Task<IActionResult> Handle(GetPublicFileQuery request, CancellationToken ct)
    {
        // Enforce this boundary here as well as in the routes, including callers
        // that send the query directly and non-filesystem storage providers.
        if (!StoragePathRules.TryNormalizeKey(request.Path, out var key) ||
            !key.StartsWith("public/", StringComparison.Ordinal))
            return new NotFoundResult();

        var file = await storage.OpenReadAsync(key, ct);
        if (file is null) 
            return new NotFoundResult();

        
        var result = new FileStreamResult(file.Stream, file.ContentType)
        {
            LastModified = file.LastModified,
            EntityTag = file.ETag is null ? null : new EntityTagHeaderValue(file.ETag),
            EnableRangeProcessing = true,
            FileDownloadName = null
        };

        return result;
    }
}

