using System.Text.Json;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.SaveOperation;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Application.Features.Resources.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

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

        var incomingTabs = cmd.Request.Tabs.Where(t => t.Status.HasValue).ToList();
        if (incomingTabs.Count == 0)
            return Result.Ok(Unit.Value);

        foreach (var noteDto in MapTabNotes(cmd.JobId, reviewCycleId, incomingTabs))
        {
            var existingNote = existingNotes.FirstOrDefault(n => n.Tab == noteDto.Tab);
            if (existingNote != null)
            {
                existingNote.UpdateNote(noteDto.Note, noteDto.TabStatus);
                existingNote.IsResolved = noteDto.TabStatus == TabStatus.Approved;
                existingNote.UpdatedDate = DateTimeOffset.UtcNow;
                continue;
            }

            await tabReviewRepo.AddAsync(noteDto);
        }

        if (!string.IsNullOrWhiteSpace(cmd.Request.AttachmentsJson))
        {
            await UpsertAttachmentAsync(cmd.JobId, reviewCycleId, cmd.Request, ct);
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }

    private async Task UpsertAttachmentAsync(Guid jobId,
        Guid reviewCycleId,
        SaveJobReviewRequestDto request,
        CancellationToken ct)
    {
        var attachmentDto = JsonSerializer.Deserialize<List<AdditionalAttachmentUpsertDto>>(
            request.AttachmentsJson!,
            JsonOptions)?.FirstOrDefault();

        if (attachmentDto == null)
        {
            Result.Ok(Unit.Value);
            return;
        }

        var uploadResult = await UploadAttachmentAsync(jobId, attachmentDto.FileIndex, request.Files, ct);
        if (uploadResult.IsFailed)
        {
            Result.Fail<Unit>(uploadResult.Errors);
            return;
        }

        var attachmentId = uploadResult.Value?.ResourceId ?? attachmentDto.AttachmentId;
        var fileName = uploadResult.Value?.ResourceName ?? attachmentDto.FileName;

        if (attachmentId is null || attachmentId == Guid.Empty || string.IsNullOrWhiteSpace(fileName))
        {
            Result.Fail<Unit>(ErrorsCodes.InvalidAttachmentFile);
            return;
        }

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
            Result.Ok(Unit.Value);
            return;
        }

        var reviewAttachment = new JobReviewAttachment
        {
            JobId = jobId,
            ReviewCycleId = reviewCycleId,
            AttachmentId = attachmentId.Value,
            FileName = fileName
        };

        await reviewAttachmentRepo.AddAsync(reviewAttachment);
        Result.Ok(Unit.Value);
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

    private static List<JobTabReviewNote> MapTabNotes(
        Guid jobId,
        Guid reviewCycleId,
        IEnumerable<JobTabReviewUpsertDto> tabs)
    {
        var createdAt = DateTimeOffset.UtcNow;
        return tabs
            .Where(t => t.Status.HasValue)
            .Select(t => new JobTabReviewNote
            {
                JobId = jobId,
                ReviewCycleId = reviewCycleId,
                Tab = t.Tab,
                Note = t.Note,
                TabStatus = t.Status!.Value,
                IsResolved = t.Status == TabStatus.Approved,
                CreatedDate = createdAt
            }).ToList();
    }
}
