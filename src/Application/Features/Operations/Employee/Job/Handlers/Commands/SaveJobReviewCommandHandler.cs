using System.Text.Json;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public sealed class SaveJobReviewCommandHandler(
    IUnitOfWork uow,
    IMediator mediator
) : IRequestHandler<SaveJobReviewCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IResult<Unit>> Handle(SaveJobReviewCommand cmd, CancellationToken ct)
    {
        var isJobExists = await uow.GetEntityRepository<JobEntity>().DbSet.AnyAsync(x => x.Id == cmd.JobId, ct);
        if (!isJobExists)
            return Result.Fail<Unit>(JobMessages.JobNotFound);

        var reviewCycleId = Guid.NewGuid();

        var tabNotes = cmd.Request.Tabs
            .Adapt<List<JobTabReviewNote>>()
            .Select(note =>
            {
                note.JobId = cmd.JobId;
                note.ReviewCycleId = reviewCycleId;
                note.IsResolved = note.TabStatus == TabStatus.Approved;
                note.CreatedDate = DateTime.UtcNow;
                return note;
            }).ToList();

        var tabReviewRepo = uow.GetEntityRepository<JobTabReviewNote>();
        await tabReviewRepo.AddRangeAsync(tabNotes);

        var reviewAttachmentRepo = uow.GetEntityRepository<JobReviewAttachment>();
        if (cmd.Request.AttachmentsJson != null)
        {
            var attachmentDto = JsonSerializer.Deserialize<List<AdditionalAttachmentUpsertDto>>(cmd.Request.AttachmentsJson, JsonOptions)?.FirstOrDefault();
            if (attachmentDto != null)
            {
                var uploadResult = await UploadAttachmentAsync(cmd.JobId, attachmentDto.FileIndex, cmd.Request.Files, ct);
                if (uploadResult.IsFailed)
                    return Result.Fail<Unit>(uploadResult.Errors);

                var attachmentId = uploadResult.Value?.ResourceId ?? attachmentDto.AttachmentId;
                var fileName = uploadResult.Value?.ResourceName ?? attachmentDto.FileName;

                if (attachmentId is null || attachmentId == Guid.Empty || string.IsNullOrWhiteSpace(fileName))
                    return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

                var reviewAttachment = new JobReviewAttachment
                {
                    JobId = cmd.JobId,
                    ReviewCycleId = reviewCycleId,
                    AttachmentId = attachmentId.Value,
                    FileName = fileName
                };
                await reviewAttachmentRepo.AddAsync(reviewAttachment);
            }
        }
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
    private async Task<Result<UploadAttachmentRequest?>> UploadAttachmentAsync(
        Guid jobId,
        int? fileIndex,
        List<IFormFile> files,
        CancellationToken ct)
    {
        if (fileIndex is null) return Result.Ok<UploadAttachmentRequest?>(null);
        if (fileIndex < 0 || fileIndex >= files.Count) return Result.Fail<UploadAttachmentRequest?>(ErrorsCodes.InvalidAttachmentFileIndex);

        var file = files[fileIndex.Value];
        if (file.Length == 0) return Result.Fail<UploadAttachmentRequest?>(ErrorsCodes.InvalidAttachmentFile);

        var uploadPath = await JobReviewUploadPathFactory.CreateAsync(jobId, file, false, ct);

        var result = await mediator.Send(new UploadAttachmentCommand(
            Guid.Empty, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file), ct);

        return result.IsFailed
            ? Result.Fail<UploadAttachmentRequest?>(result.Errors)
            : Result.Ok<UploadAttachmentRequest?>(result.Value);
    }
}
