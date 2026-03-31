using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;
using FluentValidation;
using Mapster;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class CreateJobFromPreviousCommandHandler(
    IJobRepository jobRepository,
    IJobPointsRepository jobPointsRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateJobFromPreviousCommand> validator)
    : IRequestHandler<CreateJobFromPreviousCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateJobFromPreviousCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail<Guid>(
                validationResult.Errors.Select(e => e.ErrorMessage)
            );
        }
        
        var sourceJobResult = await jobRepository.GetByIdWithDetailsAsync(request.SourceJobId);
        if (sourceJobResult.IsFailed || sourceJobResult.Value == null)
            return Result.Fail<Guid>(JobMessages.JobNotFound);

        var sourceJob = sourceJobResult.Value;
        if (!JobBusinessRules.CanCopyFromPreviousJob(sourceJob.JobStatusId))
            return Result.Fail<Guid>(JobMessages.JobCannotBeCopied);

        return await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var job = request.Job.Adapt<JobEntity>();
            ApplyOverviewFields(job, request.Job, sourceJob);
            job.ChangeStatus(JobStatusIds.Draft);

            var jobAddResult = await jobRepository.Repository.AddAsync(job, ct);
            if (jobAddResult.IsFailed)
                return Result.Fail<Guid>(jobAddResult.Errors);

            await unitOfWork.SaveChangesAsync(ct);

            // Build and insert individual objects
            var degrees = BuildDegrees(request.Job, sourceJob, job.Id);
            foreach (var degree in degrees)
            {
                var result = await unitOfWork.GetEntityRepository<JobDegree>().AddAsync(degree, ct);
                if (result.IsFailed)
                    return Result.Fail<Guid>(result.Errors);
            }
            await unitOfWork.SaveChangesAsync(ct);

            var conditions = BuildConditions(request.Job, sourceJob, job.Id);
            foreach (var condition in conditions)
            {
                var result = await unitOfWork.GetEntityRepository<JobCondition>().AddAsync(condition, ct);
                if (result.IsFailed)
                    return Result.Fail<Guid>(result.Errors);
            }
            await unitOfWork.SaveChangesAsync(ct);

            var responsibilities = BuildResponsibilities(request.Job, sourceJob, job.Id);
            foreach (var responsibility in responsibilities)
            {
                var result = await unitOfWork.GetEntityRepository<JobResponsibility>().AddAsync(responsibility, ct);
                if (result.IsFailed)
                    return Result.Fail<Guid>(result.Errors);
            }
            await unitOfWork.SaveChangesAsync(ct);

            var skills = BuildSkills(request.Job, sourceJob, job.Id);
            foreach (var skill in skills)
            {
                var result = await unitOfWork.GetEntityRepository<JobSkill>().AddAsync(skill, ct);
                if (result.IsFailed)
                    return Result.Fail<Guid>(result.Errors);
            }
            await unitOfWork.SaveChangesAsync(ct);

            var attachments = BuildRequiredAttachments(request.Job, sourceJob, job.Id);
            foreach (var a in attachments)
            {
                var result = await unitOfWork.GetEntityRepository<JobRequiredAttachment>().AddAsync(a, ct);
                if (result.IsFailed)
                    return Result.Fail<Guid>(result.Errors);
            }
            await unitOfWork.SaveChangesAsync(ct);

            var points = await BuildJobPointsAsync(
                request.Job,
                sourceJob.Id,
                sourceJob.JobPoints != null,
                job.Id);

            if (points != null)
            {
                var details = points.Details?.ToList();
                points.Details = []; // Clear details to insert them individually

                var pointsMainResult = await jobPointsRepository.Repository.AddAsync(points, ct);
                if (pointsMainResult.IsFailed)
                    return Result.Fail<Guid>(pointsMainResult.Errors);
                await unitOfWork.SaveChangesAsync(ct);

                if (details != null)
                {
                    foreach (var detail in details)
                    {
                        detail.JobPointsMainId = points.Id;
                        var detailResult = await unitOfWork.GetEntityRepository<JobPointsDetail>().AddAsync(detail, ct);
                        if (detailResult.IsFailed)
                            return Result.Fail<Guid>(detailResult.Errors);
                    }
                    await unitOfWork.SaveChangesAsync(ct);
                }
            }

            return Result.Ok(job.Id);
        }, cancellationToken);
    }

    private static void ApplyOverviewFields(JobEntity job, CreateJobFromPreviousDto request, JobEntity source)
    {
        job.OverViewAr = request.OverviewAr ?? source.OverViewAr;
        job.OverViewEn = request.OverviewEn ?? source.OverViewEn;
        job.BenefitsAr = request.BenefitsAr ?? source.BenefitsAr;
        job.BenefitsEn = request.BenefitsEn ?? source.BenefitsEn;
        job.QualificationDescriptionAr = request.QualificationsDescriptionAr ?? source.QualificationDescriptionAr;
        job.QualificationDescriptionEn = request.QualificationsDescriptionEn ?? source.QualificationDescriptionEn;
    }

    private static List<JobDegree> BuildDegrees(
        CreateJobFromPreviousDto request,
        JobEntity source,
        Guid jobId)
    {
        var degrees = request.Degrees ?? source.JobDegrees
            .Select(d => new JobDegreeRequestDto { DegreeId = d.DegreeId })
            .ToList();

        return degrees.Select(d => new JobDegree
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            DegreeId = d.DegreeId
        }).ToList();
    }

    private static List<JobCondition> BuildConditions(
        CreateJobFromPreviousDto request,
        JobEntity source,
        Guid jobId)
    {
        var conditions = request.Conditions ?? source.JobConditions
            .Select(c => new JobConditionRequestDto { TextAr = c.TextAr, TextEn = c.TextEn })
            .ToList();

        return conditions.Select(c => new JobCondition
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            TextAr = c.TextAr,
            TextEn = c.TextEn
        }).ToList();
    }

    private static List<JobResponsibility> BuildResponsibilities(
        CreateJobFromPreviousDto request,
        JobEntity source,
        Guid jobId)
    {
        var responsibilities = request.Responsibilities ?? source.JobResponsibilities
            .Select(r => new JobResponsibilityRequestDto { TextAr = r.TextAr, TextEn = r.TextEn })
            .ToList();

        return responsibilities.Select(r => new JobResponsibility
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            TextAr = r.TextAr,
            TextEn = r.TextEn
        }).ToList();
    }

    private static List<JobSkill> BuildSkills(
        CreateJobFromPreviousDto request,
        JobEntity source,
        Guid jobId)
    {
        var skills = request.Skills ?? source.JobSkills
            .Select(s => new JobSkillRequestDto { SkillId = s.SkillId, ShowToApplicants = s.ShowToApplicants })
            .ToList();

        return skills.Select(s => new JobSkill
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            SkillId = s.SkillId,
            ShowToApplicants = s.ShowToApplicants
        }).ToList();
    }

    private static List<JobRequiredAttachment> BuildRequiredAttachments(
        CreateJobFromPreviousDto request,
        JobEntity source,
        Guid jobId)
    {
        var attachments = request.RequiredAttachments ?? source.JobRequiredAttachments
            .Select(a => new JobRequiredAttachmentRequestDto
            {
                TitleAr = a.TitleAr,
                TitleEn = a.TitleEn,
                IsMandatory = a.IsMandatory
            }).ToList();

        return attachments.Select(a => new JobRequiredAttachment
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            TitleAr = a.TitleAr,
            TitleEn = a.TitleEn,
            IsMandatory = a.IsMandatory
        }).ToList();
    }

    private async Task<JobPointsMain?> BuildJobPointsAsync(
        CreateJobFromPreviousDto request,
        Guid sourceJobId,
        bool sourceHasPoints,
        Guid jobId)
    {
        JobPointsCopyDto? points = request.JobPoints;

        if (points == null && sourceHasPoints)
        {
            var sourcePointsResult = await jobPointsRepository.GetByJobIdAsync(sourceJobId);
            if (sourcePointsResult.IsSuccess)
                points = sourcePointsResult.Value.Adapt<JobPointsCopyDto>();
        }

        if (points == null)
            return null;

        var details = points.Details;

        return new JobPointsMain
        {
            Id = Guid.NewGuid(),
            JobId = jobId,
            ApplicantCategory = points.ApplicantCategory,
            Education = points.Education,
            Experience = points.Experience,
            Training = points.Training,
            Skills = points.Skills,
            Languages = points.Languages,
            Certificates = points.Certificates,
            Total = points.Total,
            IsApproved = false,
            Details = details.Select(d => new JobPointsDetail
            {
                Id = Guid.NewGuid(),
                Type = d.Type,
                Code = d.Code,
                Name = d.Name,
                ReferenceId = d.ReferenceId,
                Points = d.Points
            }).ToList()
        };
    }
}

