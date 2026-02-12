using Cortex.Mediator.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Features.Resources.Queries;

namespace Tawtheef.Application.Features.Resources.Handlers.Queries;

public sealed class GetPublicFileHandler(IFileStorageService storage) : IQueryHandler<GetPublicFileQuery, IActionResult>
{
    public async Task<IActionResult> Handle(GetPublicFileQuery request, CancellationToken ct)
    {
        var file = await storage.OpenReadAsync(request.Path, ct);
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
