using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public class UploadAttachmentHandler(
    IUnitOfWork uow,
    IFileStorageService fileStorageService)
: IRequestHandler<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>
{
    public async Task<IResult<UploadAttachmentRequest>> Handle(UploadAttachmentCommand cmd, CancellationToken ct)
    {
        if (cmd.file.Length == 0) return Result.Fail<UploadAttachmentRequest>(ErrorsCodes.EmptyFile);
        await using var stream = cmd.file.OpenReadStream();
        var result = await fileStorageService.SaveAsync(stream, Guid.NewGuid().ToString(), ct);
        if (result.IsFailed) return Result.Fail<UploadAttachmentRequest>(result.Errors);

        var uploadResult = await uow.GetEntityRepository<Resource>().AddAsync(new Resource
        {
            Name = cmd.file.FileName,
            Url = result.Value.BlobKey,
            Key = result.Value.BlobKey,
            Size = result.Value.Size,
            Type = cmd.file.ContentType
        });
        if (uploadResult.IsFailed) return Result.Fail<UploadAttachmentRequest>(uploadResult.Errors);
        await uow.SaveChangesAsync(ct).ConfigureAwait(false);
        return Result.Ok(new UploadAttachmentRequest(uploadResult.Value.Id, uploadResult.Value.Name));
    }
}
