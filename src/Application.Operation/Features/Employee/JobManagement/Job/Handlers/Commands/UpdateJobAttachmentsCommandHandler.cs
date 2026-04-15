using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

using Application.Operation.Common.Validations;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class UpdateJobAttachmentsCommandHandler(
    IJobValidationService validationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobAttachmentsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobAttachmentsCommand request, CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<JobEntity>().DbSet
            .Include(j => j.JobRequiredAttachments)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        
        if (job is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        var validationResult = await validationService.ValidateAttachmentsUpdate(request.Data, job);
        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.First().ErrorMessage);

        // 4. Sync required attachments collection
        await SyncRequiredAttachments(unitOfWork, job.Id, job.JobRequiredAttachments, request.Data.RequiredAttachments, cancellationToken);

        // 5. Persist
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
    private async Task SyncRequiredAttachments(IUnitOfWork uow,Guid jobId, List<JobRequiredAttachment> existsAttachments, List<JobRequiredAttachmentRequestDto> newAttachments, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<JobRequiredAttachment>();
        // Deduplicate by (TitleAr, TitleEn) composite key
        var distinctNew = newAttachments
            .GroupBy(a => (a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()))
            .Select(g => g.First())
            .ToList();

        var desiredKeys = distinctNew
            .Select(a => (a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()))
            .ToHashSet();

        // Remove items not in the new set
        var toRemove = existsAttachments
            .Where(a => !desiredKeys.Contains((a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant())))
            .ToList();
        foreach (var item in toRemove)
            await repo.DeleteAsync(item.Id,ct);

        // Update IsMandatory for existing items
        var newLookup = distinctNew.ToDictionary(
            a => (a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()));
        foreach (var existing in existsAttachments)
        {
            var key = (existing.TitleAr.ToUpperInvariant(), existing.TitleEn.ToUpperInvariant());
            if (newLookup.TryGetValue(key, out var dto))
                existing.IsMandatory = dto.IsMandatory;
        }

        // Track existing keys for Add check
        var existingKeys = existsAttachments
            .Select(a => (a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()))
            .ToHashSet();

        // Add new items
        foreach (var dto in distinctNew.Where(a =>
            !existingKeys.Contains((a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()))))
        {
            await repo.AddAsync(new JobRequiredAttachment
            {
                JobId = jobId,
                TitleAr = dto.TitleAr,
                TitleEn = dto.TitleEn,
                IsMandatory = dto.IsMandatory
            }, ct);
        }
    }
}
