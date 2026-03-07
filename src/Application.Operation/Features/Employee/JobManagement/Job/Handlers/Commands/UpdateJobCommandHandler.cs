using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;
using FluentValidation;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class UpdateJobCommandHandler(
    IJobRepository jobRepository,
    IJobSkillRepository jobSkillRepository,
    IJobConditionRepository jobConditionRepository,
    IJobDegreeRepository jobDegreeRepository,
    IJobResponsibilityRepository jobResponsibilityRepository,
    IJobRequiredAttachmentRepository jobRequiredAttachmentRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateJobCommand> validator
    )
    : IRequestHandler<UpdateJobCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail<Unit>(
                validationResult.Errors.Select(e => e.ErrorMessage)
            );
        }
        var existingJobResult = await jobRepository.GetByIdWithDetailsAsync(request.JobId);
        if (existingJobResult.IsFailed || existingJobResult.Value == null)
            return Result.Fail<Unit>(JobMessages.JobNotFound);

        var existingJob = existingJobResult.Value;

        if (!JobBusinessRules.CanEdit(existingJob.JobStatusId))
            return Result.Fail<Unit>(JobMessages.CanOnlyEditInDraft);

        if (request.Job.Skills != null)
            await UpdateSkillsAsync(existingJob, request.Job.Skills);

        if (request.Job.Conditions != null)
            await UpdateConditionsAsync(existingJob, request.Job.Conditions);

        if (request.Job.Degrees != null)
            await UpdateDegreesAsync(existingJob, request.Job.Degrees.Select(d => d.DegreeId).ToList());

        if (request.Job.Responsibilities != null)
            await UpdateResponsibilitiesAsync(existingJob, request.Job.Responsibilities);

        if (request.Job.RequiredAttachments != null)
            await UpdateRequiredAttachmentsAsync(existingJob, request.Job.RequiredAttachments);

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
        await jobRepository.Repository.UpdateAsync(existingJob);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }

    private async Task UpdateSkillsAsync(JobEntity job, List<JobSkillRequestDto> newSkills)
    {
        var existingSkills = job.JobSkills.ToList();

        var toRemove = existingSkills
            .Where(s => newSkills.All(ns => ns.SkillId != s.SkillId))
            .ToList();

        var existingIds = existingSkills.Select(s => s.SkillId).ToHashSet();

        var toAdd = newSkills
            .Where(ns => !existingIds.Contains(ns.SkillId))
            .Select(ns => new JobSkill
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                SkillId = ns.SkillId,
                ShowToApplicants = ns.ShowToApplicants
            })
            .ToList();

        foreach (var item in toRemove)
            job.JobSkills.Remove(item);

        foreach (var item in toAdd)
            job.JobSkills.Add(item);

        if (toRemove.Count != 0)
            await jobSkillRepository.Repository.DeleteRangeAsync(toRemove);

        if (toAdd.Count != 0)
            await jobSkillRepository.Repository.AddRangeAsync(toAdd);
    }

    private async Task UpdateConditionsAsync(JobEntity job, List<JobConditionRequestDto> newConditions)
    {
        var existing = job.JobConditions.ToList();

        var toRemove = existing
            .Where(c => newConditions.All(n => n.TextAr != c.TextAr))
            .ToList();

        var existingTexts = existing.Select(c => c.TextAr).ToHashSet();

        var toAdd = newConditions
            .Where(nc => !existingTexts.Contains(nc.TextAr))
            .Select(nc => new JobCondition
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TextAr = nc.TextAr,
                TextEn = nc.TextEn
            })
            .ToList();

        foreach (var rm in toRemove)
            job.JobConditions.Remove(rm);

        foreach (var add in toAdd)
            job.JobConditions.Add(add);

        if (toRemove.Count != 0)
            await jobConditionRepository.Repository.DeleteRangeAsync(toRemove);

        if (toAdd.Count != 0)
            await jobConditionRepository.Repository.AddRangeAsync(toAdd);
    }

    private async Task UpdateDegreesAsync(JobEntity job, List<Guid> newDegreeIds)
    {
        var existing = job.JobDegrees.ToList();

        var toRemove = existing.Where(d => !newDegreeIds.Contains(d.DegreeId)).ToList();

        var existingIds = existing.Select(d => d.DegreeId).ToHashSet();

        var toAdd = newDegreeIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => new JobDegree
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                DegreeId = id
            })
            .ToList();

        foreach (var rm in toRemove)
            job.JobDegrees.Remove(rm);

        foreach (var add in toAdd)
            job.JobDegrees.Add(add);

        if (toRemove.Count != 0)
            await jobDegreeRepository.Repository.DeleteRangeAsync(toRemove);

        if (toAdd.Count != 0)
            await jobDegreeRepository.Repository.AddRangeAsync(toAdd);
    }


    private async Task UpdateResponsibilitiesAsync(JobEntity job, List<JobResponsibilityRequestDto> newResponsibilities)
    {
        var existing = job.JobResponsibilities.ToList();

        var toRemove = existing
            .Where(r => newResponsibilities.All(nr => nr.TextAr != r.TextAr))
            .ToList();

        var existingTitles = existing.Select(r => r.TextAr).ToHashSet();

        var toAdd = newResponsibilities
            .Where(r => !existingTitles.Contains(r.TextAr))
            .Select(r => new JobResponsibility
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TextAr = r.TextAr,
                TextEn = r.TextEn
            })
            .ToList();

        foreach (var rm in toRemove)
            job.JobResponsibilities.Remove(rm);

        foreach (var add in toAdd)
            job.JobResponsibilities.Add(add);

        if (toRemove.Count != 0)
            await jobResponsibilityRepository.Repository.DeleteRangeAsync(toRemove);

        if (toAdd.Count != 0)
            await jobResponsibilityRepository.Repository.AddRangeAsync(toAdd);
    }


    private async Task UpdateRequiredAttachmentsAsync(JobEntity job, List<JobRequiredAttachmentRequestDto> newAttachments)
    {
        var existing = job.JobRequiredAttachments.ToList();

        var toRemove = existing
            .Where(a => !newAttachments.Any(na =>
                na.TitleAr == a.TitleAr && na.TitleEn == a.TitleEn))
            .ToList();

        var existingKeys = existing
            .Select(a => (a.TitleAr, a.TitleEn))
            .ToHashSet();

        var toAdd = newAttachments
            .Where(a => !existingKeys.Contains((a.TitleAr, a.TitleEn)))
            .Select(a => new JobRequiredAttachment
            {
                Id = Guid.NewGuid(),
                JobId = job.Id,
                TitleAr = a.TitleAr,
                TitleEn = a.TitleEn,
                IsMandatory = a.IsMandatory
            })
            .ToList();

        foreach (var rm in toRemove)
            job.JobRequiredAttachments.Remove(rm);

        foreach (var add in toAdd)
            job.JobRequiredAttachments.Add(add);

        if (toRemove.Count != 0)
            await jobRequiredAttachmentRepository.Repository.DeleteRangeAsync(toRemove);

        if (toAdd.Count != 0)
            await jobRequiredAttachmentRepository.Repository.AddRangeAsync(toAdd);
    }

}

