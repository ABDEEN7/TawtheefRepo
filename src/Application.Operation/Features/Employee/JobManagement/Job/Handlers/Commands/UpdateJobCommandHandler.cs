using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class UpdateJobCommandHandler(
    IJobRepository jobRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateJobCommand> validator
    )
    : IRequestHandler<UpdateJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate command (business rules + referential integrity via IJobValidationService)
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail<Unit>(
                validationResult.Errors.Select(e => e.ErrorMessage)
            );
        }

        // 2. Load the aggregate root with all child collections (tracked by EF)
        var existingJobResult = await jobRepository.GetByIdWithDetailsAsync(request.JobId);
        if (existingJobResult.IsFailed || existingJobResult.Value == null)
            return Result.Fail<Unit>(JobMessages.JobNotFound);

        var existingJob = existingJobResult.Value;

        // 3. Enforce business rule: can only edit in Draft / NeedUpdate
        if (!JobBusinessRules.CanEdit(existingJob.JobStatusId))
            return Result.Fail<Unit>(JobMessages.CanOnlyEditInDraft);

        // 4. Handle Concurrency: Map RowVersion from DTO to tracked entity
        if (request.Job.RowVersion is { Length: > 0 })
        {
            unitOfWork.Context.Entry(existingJob).Property(j => j.RowVersion).OriginalValue = request.Job.RowVersion;
        }

        // 5. Sync child collections via EF change tracking only (no double-writes)
        if (request.Job.Skills != null)
            SyncSkills(existingJob, request.Job.Skills);

        if (request.Job.Conditions != null)
            SyncConditions(existingJob, request.Job.Conditions);

        if (request.Job.Degrees != null)
            SyncDegrees(existingJob, request.Job.Degrees.Select(d => d.DegreeId).ToList());

        if (request.Job.Responsibilities != null)
            SyncResponsibilities(existingJob, request.Job.Responsibilities);

        if (request.Job.RequiredAttachments != null)
            SyncRequiredAttachments(existingJob, request.Job.RequiredAttachments);

        if (request.Job.JobSpecializations != null)
            SyncSpecializations(existingJob, request.Job.JobSpecializations);

        // 5. Update scalar properties
        existingJob.JobTitleId = request.Job.JobTitleId;
        existingJob.SectorId = request.Job.SectorId;
        existingJob.ManagementId = request.Job.ManagementId;
        existingJob.DepartmentId = request.Job.DepartmentId;
        existingJob.YearsOfExperience = request.Job.YearsOfExperience;
        existingJob.JobCategoryId = request.Job.JobCategoryId;
        existingJob.WorkLocationId = request.Job.WorkLocationId;
        existingJob.GenderId = request.Job.GenderId;
        existingJob.MajorId = request.Job.MajorId;
        existingJob.SubMajorId = request.Job.SubMajorId;
        existingJob.WorkTypeId = request.Job.WorkTypeId;
        existingJob.NumberOfVacancies = request.Job.NumberOfVacancies;
        existingJob.ClosingDate = request.Job.ClosingDate.UtcDateTime;
        existingJob.MinimumAge = request.Job.MinimumAge;
        existingJob.MaximumAge = request.Job.MaximumAge;
        existingJob.OverViewAr = request.Job.OverviewAr;
        existingJob.OverViewEn = request.Job.OverviewEn;
        existingJob.BenefitsAr = request.Job.BenefitsAr;
        existingJob.BenefitsEn = request.Job.BenefitsEn;
        existingJob.QualificationDescriptionAr = request.Job.QualificationsDescriptionAr;
        existingJob.QualificationDescriptionEn = request.Job.QualificationsDescriptionEn;

        // 6. Persist all changes in a single SaveChanges call (single transaction boundary)
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Fail<Unit>(JobMessages.ConcurrencyConflict);
        }

        return Result.Ok(Unit.Value);
    }

    // ──────────────────────────────────────────────
    //  Collection Sync Methods
    //  - Rely purely on EF change tracking (no explicit repository Add/Delete calls)
    //  - Deduplicate input upfront
    //  - Use HashSet for O(1) lookups instead of O(n²) Any()
    // ──────────────────────────────────────────────

    private static void SyncSkills(JobEntity job, List<JobSkillRequestDto> newSkills)
    {
        // Deduplicate input by SkillId (keep first occurrence)
        var distinctNew = newSkills
            .GroupBy(s => s.SkillId)
            .Select(g => g.First())
            .ToList();

        var desiredIds = distinctNew.Select(s => s.SkillId).ToHashSet();
        var existingIds = job.JobSkills.Select(s => s.SkillId).ToHashSet();

        // Remove items not in the new set
        var toRemove = job.JobSkills.Where(s => !desiredIds.Contains(s.SkillId)).ToList();
        foreach (var item in toRemove)
            job.JobSkills.Remove(item);

        // Update ShowToApplicants for existing items
        var newLookup = distinctNew.ToDictionary(s => s.SkillId);
        foreach (var existing in job.JobSkills)
        {
            if (newLookup.TryGetValue(existing.SkillId, out var dto))
                existing.ShowToApplicants = dto.ShowToApplicants;
        }

        // Add new items
        foreach (var dto in distinctNew.Where(s => !existingIds.Contains(s.SkillId)))
        {
            job.JobSkills.Add(new JobSkill
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                SkillId = dto.SkillId,
                ShowToApplicants = dto.ShowToApplicants
            });
        }
    }

    private static void SyncConditions(JobEntity job, List<JobConditionRequestDto> newConditions)
    {
        // Deduplicate by TextAr (case-insensitive)
        var distinctNew = newConditions
            .GroupBy(c => c.TextAr, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        var desiredKeys = distinctNew.Select(c => c.TextAr).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingKeys = job.JobConditions.Select(c => c.TextAr).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Remove items not in the new set
        var toRemove = job.JobConditions.Where(c => !desiredKeys.Contains(c.TextAr)).ToList();
        foreach (var item in toRemove)
            job.JobConditions.Remove(item);

        // Update TextEn for existing items
        var newLookup = distinctNew.ToDictionary(c => c.TextAr, c => c, StringComparer.OrdinalIgnoreCase);
        foreach (var existing in job.JobConditions)
        {
            if (newLookup.TryGetValue(existing.TextAr, out var dto))
                existing.TextEn = dto.TextEn;
        }

        // Add new items
        foreach (var dto in distinctNew.Where(c => !existingKeys.Contains(c.TextAr)))
        {
            job.JobConditions.Add(new JobCondition
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TextAr = dto.TextAr,
                TextEn = dto.TextEn
            });
        }
    }

    private static void SyncDegrees(JobEntity job, List<Guid> newDegreeIds)
    {
        // Deduplicate
        var desiredIds = newDegreeIds.Distinct().ToHashSet();
        var existingIds = job.JobDegrees.Select(d => d.DegreeId).ToHashSet();

        // Remove items not in the new set
        var toRemove = job.JobDegrees.Where(d => !desiredIds.Contains(d.DegreeId)).ToList();
        foreach (var item in toRemove)
            job.JobDegrees.Remove(item);

        // Add new items
        foreach (var id in desiredIds.Where(id => !existingIds.Contains(id)))
        {
            job.JobDegrees.Add(new JobDegree
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                DegreeId = id
            });
        }
    }

    private static void SyncResponsibilities(JobEntity job, List<JobResponsibilityRequestDto> newResponsibilities)
    {
        // Deduplicate by TextAr (case-insensitive)
        var distinctNew = newResponsibilities
            .GroupBy(r => r.TextAr, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        var desiredKeys = distinctNew.Select(r => r.TextAr).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingKeys = job.JobResponsibilities.Select(r => r.TextAr).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Remove items not in the new set
        var toRemove = job.JobResponsibilities.Where(r => !desiredKeys.Contains(r.TextAr)).ToList();
        foreach (var item in toRemove)
            job.JobResponsibilities.Remove(item);

        // Update TextEn for existing items
        var newLookup = distinctNew.ToDictionary(r => r.TextAr, r => r, StringComparer.OrdinalIgnoreCase);
        foreach (var existing in job.JobResponsibilities)
        {
            if (newLookup.TryGetValue(existing.TextAr, out var dto))
                existing.TextEn = dto.TextEn;
        }

        // Add new items
        foreach (var dto in distinctNew.Where(r => !existingKeys.Contains(r.TextAr)))
        {
            job.JobResponsibilities.Add(new JobResponsibility
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TextAr = dto.TextAr,
                TextEn = dto.TextEn
            });
        }
    }

    private static void SyncRequiredAttachments(JobEntity job, List<JobRequiredAttachmentRequestDto> newAttachments)
    {
        // Deduplicate by (TitleAr, TitleEn) composite key
        var distinctNew = newAttachments
            .GroupBy(a => (a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()))
            .Select(g => g.First())
            .ToList();

        var desiredKeys = distinctNew
            .Select(a => (a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()))
            .ToHashSet();

        // Remove items not in the new set
        var toRemove = job.JobRequiredAttachments
            .Where(a => !desiredKeys.Contains((a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant())))
            .ToList();
        foreach (var item in toRemove)
            job.JobRequiredAttachments.Remove(item);

        // Update IsMandatory for existing items
        var newLookup = distinctNew.ToDictionary(
            a => (a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()));
        foreach (var existing in job.JobRequiredAttachments)
        {
            var key = (existing.TitleAr.ToUpperInvariant(), existing.TitleEn.ToUpperInvariant());
            if (newLookup.TryGetValue(key, out var dto))
                existing.IsMandatory = dto.IsMandatory;
        }

        // Track existing keys for Add check
        var existingKeys = job.JobRequiredAttachments
            .Select(a => (a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()))
            .ToHashSet();

        // Add new items
        foreach (var dto in distinctNew.Where(a =>
            !existingKeys.Contains((a.TitleAr.ToUpperInvariant(), a.TitleEn.ToUpperInvariant()))))
        {
            job.JobRequiredAttachments.Add(new JobRequiredAttachment
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TitleAr = dto.TitleAr,
                TitleEn = dto.TitleEn,
                IsMandatory = dto.IsMandatory
            });
        }
    }

    private static void SyncSpecializations(JobEntity job, List<JobSpecializationRequestDto> newSpecs)
    {
        // Deduplicate by (MajorId, SubMajorId) composite key
        var distinctNew = newSpecs
            .GroupBy(s => (s.MajorId, s.SubMajorId))
            .Select(g => g.First())
            .ToList();

        var desiredKeys = distinctNew
            .Select(s => (s.MajorId, s.SubMajorId))
            .ToHashSet();

        var existingKeys = job.JobSpecializations
            .Select(s => (s.MajorId, s.SubMajorId))
            .ToHashSet();

        // Remove items not in the new set
        var toRemove = job.JobSpecializations
            .Where(s => !desiredKeys.Contains((s.MajorId, s.SubMajorId)))
            .ToList();
        foreach (var item in toRemove)
            job.JobSpecializations.Remove(item);

        // Add new items
        foreach (var dto in distinctNew.Where(s => !existingKeys.Contains((s.MajorId, s.SubMajorId))))
        {
            job.JobSpecializations.Add(new JobSpecialization
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                MajorId = dto.MajorId,
                SubMajorId = dto.SubMajorId
            });
        }
    }
}
