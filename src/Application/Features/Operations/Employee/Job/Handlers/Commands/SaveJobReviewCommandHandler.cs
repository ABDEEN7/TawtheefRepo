using System.Text.Json;
using FluentResults;
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
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IResult<Unit>> Handle(
        SaveJobReviewCommand cmd,
        CancellationToken ct)
    {
        var jobRepo = uow.GetEntityRepository<JobEntity>();

        var job = await jobRepo.DbSet
            .FirstOrDefaultAsync(x => x.Id == cmd.JobId, ct);

        if (job is null)
            return Result.Fail<Unit>(JobMessages.JOB_NOT_FOUND);

        var reviewCycleId = Guid.NewGuid();

        IReadOnlyList<IFormFile> files =
            cmd.Request.Files is { Count: > 0 }
                ? cmd.Request.Files
                : Array.Empty<IFormFile>();

        foreach (var tabDto in cmd.Request.Tabs)
        {
            var note = new JobTabReviewNote
            {
                JobId = job.Id,
                ReviewCycleId = reviewCycleId, 
                Tab = tabDto.Tab,
                Note = tabDto.Note,
                TabStatus = tabDto.Status,
                IsResolved = TabStatus.Approved == tabDto.Status, 
                CreatedDate = DateTime.UtcNow
            };

            var attachmentsResult = Deserialize(tabDto.AttachmentsJson);
            if (attachmentsResult.IsFailed)
                return Result.Fail<Unit>(attachmentsResult.Errors);

            foreach (var dto in attachmentsResult.Value)
            {
                var uploadResult = await UploadIfNeededAsync(
                    job.Id,
                    tabDto.Tab,
                    dto.FileIndex,
                    files,
                    ct);

                if (uploadResult.IsFailed)
                    return Result.Fail<Unit>(uploadResult.Errors);

                var attachmentId = uploadResult.Value?.ResourceId ?? dto.AttachmentId;
                var fileName = uploadResult.Value?.ResourceName ?? dto.FileName;

                if (attachmentId is null || attachmentId == Guid.Empty ||
                    string.IsNullOrWhiteSpace(fileName))
                    return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

                note.Attachments ??= new List<JobTabReviewAttachment>();

                note.Attachments.Add(new JobTabReviewAttachment
                {
                    FileName = fileName,
                    AttachmentId = attachmentId.Value,
                    JobTabReviewNote = note
                });
            }

            job.TabReviewNotes.Add(note);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private static Result<List<AdditionalAttachmentUpsertDto>> Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return Result.Ok(new List<AdditionalAttachmentUpsertDto>());

        try
        {
            return Result.Ok(
                JsonSerializer.Deserialize<List<AdditionalAttachmentUpsertDto>>(json, JsonOptions)
                ?? new List<AdditionalAttachmentUpsertDto>()
            );
        }
        catch
        {
            return Result.Fail<List<AdditionalAttachmentUpsertDto>>(ErrorsCodes.InvalidAttachmentsJson);
        }
    }

    private async Task<Result<UploadAttachmentRequest?>> UploadIfNeededAsync(
        Guid jobId,
        TabType tab,
        int? fileIndex,
        IReadOnlyList<IFormFile> files,
        CancellationToken ct)
    {
        if (fileIndex is null)
            return Result.Ok<UploadAttachmentRequest?>(null);

        if (fileIndex < 0 || fileIndex >= files.Count)
            return Result.Fail<UploadAttachmentRequest?>(ErrorsCodes.InvalidAttachmentFileIndex);

        var file = files[fileIndex.Value];
        if (file is not { Length: > 0 })
            return Result.Fail<UploadAttachmentRequest?>(ErrorsCodes.InvalidAttachmentFile);

        var uploadPath = await JobReviewUploadPathFactory.CreateAsync(
            jobId,
            tab,
            file,
            false,
            ct);

        var uploadResult = await mediator.Send(
            new UploadAttachmentCommand(
                Guid.Empty,
                uploadPath.FileId,
                uploadPath.Path,
                uploadPath.Hash,
                file),
            ct);

        return uploadResult.IsFailed
            ? Result.Fail<UploadAttachmentRequest?>(uploadResult.Errors)
            : Result.Ok<UploadAttachmentRequest?>(uploadResult.Value);
    }
}
