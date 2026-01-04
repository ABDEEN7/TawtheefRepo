using FluentResults;
using Mapster;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public class CreateJobFromPreviousCommandHandler(
    IJobRepository jobRepository,
    IJobPointsRepository jobPointsRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobFromPreviousCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateJobFromPreviousCommand request,
        CancellationToken cancellationToken)
    {
        var sourceJobResult = await jobRepository.GetByIdWithDetailsAsync(request.SourceJobId);
        if (sourceJobResult.IsFailed || sourceJobResult.Value == null)
            return Result.Fail<Guid>(JobMessages.JobNotFound);

        var sourceJob = sourceJobResult.Value;
        if (!JobBusinessRules.CanCopyFromPreviousJob(sourceJob.JobStatusId))
            return Result.Fail<Guid>(JobMessages.JobCannotBeCopied);

        var job = request.Job.Adapt<JobEntity>();
        
        ApplyOverviewFields(job, request.Job, sourceJob);
        job.JobDegrees = BuildDegrees(request.Job, sourceJob, job.Id);
        job.JobConditions = BuildConditions(request.Job, sourceJob, job.Id);
        job.JobResponsibilities = BuildResponsibilities(request.Job, sourceJob, job.Id);
        job.JobSkills = BuildSkills(request.Job, sourceJob, job.Id);
        job.JobRequiredAttachments = BuildRequiredAttachments(request.Job, sourceJob, job.Id);

        var points = await BuildJobPointsAsync(
            request.Job,
            sourceJob.Id,
            sourceJob.JobPoints != null,
            job.Id);
        if (points != null)
            job.JobPoints = points;

        job.ChangeStatus(JobStatusIds.Draft);

        var result = await jobRepository.Repository.AddAsync(job);
        if (result.IsFailed)
            return Result.Fail<Guid>(result.Errors);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(job.Id);
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
