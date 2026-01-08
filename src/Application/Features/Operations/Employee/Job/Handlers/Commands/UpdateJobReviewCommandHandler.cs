using System.Text.Json;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public sealed class UpdateJobReviewCommandHandler(
    IUnitOfWork uow,
    IMediator mediator
) : ICommandHandler<UpdateJobReviewCommand, IResult<Unit>>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IResult<Unit>> Handle(UpdateJobReviewCommand cmd, CancellationToken ct)
    {
        var isJobExists = await uow.GetEntityRepository<JobEntity>().DbSet.AnyAsync(x => x.Id == cmd.JobId, ct);
        if (!isJobExists)
            return Result.Fail<Unit>(JobMessages.JobNotFound);

        var tabReviewRepo = uow.GetEntityRepository<JobTabReviewNote>();
        var lastCycle = await tabReviewRepo.DbSet
            .AsNoTracking()
            .Where(x => x.JobId == cmd.JobId)
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefaultAsync(ct);

        var reviewCycleId = lastCycle?.ReviewCycleId ?? Guid.NewGuid();

        var existingNotes = await tabReviewRepo.DbSet
            .Where(x => x.JobId == cmd.JobId && x.ReviewCycleId == reviewCycleId)
            .ToListAsync(ct);

        foreach (var noteDto in cmd.Request.Tabs.Adapt<List<JobTabReviewNote>>())
        {
            var existingNote = existingNotes.FirstOrDefault(n => n.Tab == noteDto.Tab);
            if (existingNote != null)
            {
                existingNote.UpdateNote(noteDto.Note, noteDto.TabStatus);
                existingNote.IsResolved = noteDto.TabStatus == TabStatus.Approved;
                existingNote.UpdatedDate = DateTimeOffset.UtcNow;
                continue;
            }

            noteDto.JobId = cmd.JobId;
            noteDto.ReviewCycleId = reviewCycleId;
            noteDto.IsResolved = noteDto.TabStatus == TabStatus.Approved;
            noteDto.CreatedDate = DateTimeOffset.UtcNow;
            await tabReviewRepo.AddAsync(noteDto);
        }

        if (!string.IsNullOrWhiteSpace(cmd.Request.AttachmentsJson))
        {
            await UpsertAttachmentAsync(cmd.JobId, reviewCycleId, cmd.Request, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private async Task<IResult<Unit>> UpsertAttachmentAsync(
        Guid jobId,
        Guid reviewCycleId,
        SaveJobReviewRequestDto request,
        CancellationToken ct)
    {
        var attachmentDto = JsonSerializer.Deserialize<List<AdditionalAttachmentUpsertDto>>(
            request.AttachmentsJson!,
            JsonOptions)?.FirstOrDefault();

        if (attachmentDto == null)
            return Result.Ok(Unit.Value);

        var uploadResult = await UploadAttachmentAsync(jobId, attachmentDto.FileIndex, request.Files, ct);
        if (uploadResult.IsFailed)
            return Result.Fail<Unit>(uploadResult.Errors);

        var attachmentId = uploadResult.Value?.ResourceId ?? attachmentDto.AttachmentId;
        var fileName = uploadResult.Value?.ResourceName ?? attachmentDto.FileName;

        if (attachmentId is null || attachmentId == Guid.Empty || string.IsNullOrWhiteSpace(fileName))
            return Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);

        var reviewAttachmentRepo = uow.GetEntityRepository<JobReviewAttachment>();
        var existingAttachment = await reviewAttachmentRepo.DbSet
            .FirstOrDefaultAsync(x =>
                x.JobId == jobId &&
                x.ReviewCycleId == reviewCycleId &&
                x.AttachmentId == attachmentId, ct);

        if (existingAttachment != null)
        {
            existingAttachment.FileName = fileName;
            existingAttachment.UpdatedDate = DateTimeOffset.UtcNow;
            return Result.Ok(Unit.Value);
        }

        var reviewAttachment = new JobReviewAttachment
        {
            JobId = jobId,
            ReviewCycleId = reviewCycleId,
            AttachmentId = attachmentId.Value,
            FileName = fileName
        };

        await reviewAttachmentRepo.AddAsync(reviewAttachment);
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

        var result = await mediator.SendCommandAsync<UploadAttachmentCommand, IResult<UploadAttachmentRequest>>(new UploadAttachmentCommand(
            Guid.Empty, uploadPath.FileId, uploadPath.Path, uploadPath.Hash, file), ct);

        return result.IsFailed
            ? Result.Fail<UploadAttachmentRequest?>(result.Errors)
            : Result.Ok<UploadAttachmentRequest?>(result.Value);
    }
}
