using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Recruitment.JobDetails.DTOs;
using Tawtheef.Application.Features.Recruitment.JobDetails.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.JobDetails.Handlers.Queries;

public sealed class GetCandidateJobDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IJobRepository jobRepository,
    IMapper mapper,
    ILocalizationService localizationService)
    : IQueryHandler<GetCandidateJobDetailsQuery, IResult<CandidateJobDetailsDto>>
{
    public async Task<IResult<CandidateJobDetailsDto>> Handle(GetCandidateJobDetailsQuery query, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                inv => inv.Id == query.InvitationId && inv.ApplicantId == query.UserId,
                cancellationToken);

        if (invitation is null)
            return Result.Fail<CandidateJobDetailsDto>(ErrorsCodes.InvitationNotFound);

        var jobResult = await jobRepository.GetByIdWithDetailsAsync(invitation.JobId);

        if (jobResult.IsFailed)
            return Result.Fail<CandidateJobDetailsDto>(jobResult.Errors);

        var job = jobResult.Value;

        if (job is null)
            return Result.Fail<CandidateJobDetailsDto>(JobMessages.JobNotFound);

        var jobDto = new CandidateJobDetailsDto
        {
            Id = job.Id,
            Title = localizationService.GetLocalizedValue(job.TitleAr ?? string.Empty, job.TitleEn ?? string.Empty),
            NumberOfVacancies = job.NumberOfVacancies,
            ClosingDate = job.ClosingDate,
            Benefits = localizationService.GetLocalizedValue(job.BenefitsAr ?? string.Empty, job.BenefitsEn ?? string.Empty),
            OverView = localizationService.GetLocalizedValue(job.OverViewAr ?? string.Empty, job.OverViewEn ?? string.Empty),
            QualificationDescription = localizationService.GetLocalizedValue(
                job.QualificationDescriptionAr ?? string.Empty,
                job.QualificationDescriptionEn ?? string.Empty),
            PublishAt = job.PublishAt,
            CreatedDate = job.CreatedDate,
            ModifiedDate = job.UpdatedDate,
            YearsOfExperience = job.YearsOfExperience,
            MinimumAge = job.MinimumAge,
            MaximumAge = job.MaximumAge,
            JobPoints = job.JobPoints is null ? null : mapper.Map<JobPointsMainResponseDto>(job.JobPoints),
            Sector = job.Sector is null ? null : mapper.Map<DropdownOptions>(job.Sector),
            Management = job.Management is null ? null : mapper.Map<DropdownOptions>(job.Management),
            Department = job.Department is null ? null : mapper.Map<DropdownOptions>(job.Department),
            JobCategory = job.JobCategory is null ? null : mapper.Map<DropdownOptions>(job.JobCategory),
            Gender = job.Gender is null ? null : mapper.Map<DropdownOptions>(job.Gender),
            WorkLocation = job.WorkLocation is null ? null : mapper.Map<DropdownOptions>(job.WorkLocation),
            Major = job.Major is null ? null : mapper.Map<DropdownOptions>(job.Major),
            SubMajor = job.SubMajor is null ? null : mapper.Map<DropdownOptions>(job.SubMajor),
            WorkType = job.WorkType is null ? null : mapper.Map<DropdownOptions>(job.WorkType),
            JobStatus = job.JobStatus is null ? null : mapper.Map<DropdownOptions>(job.JobStatus),
            Degrees = job.JobDegrees is null ? [] : mapper.Map<List<JobDegreeResponseDto>>(job.JobDegrees),
            Conditions = job.JobConditions is null
                ? []
                : job.JobConditions.Select(condition => new CandidateJobConditionDto
                {
                    Id = condition.Id,
                    JobId = condition.JobId,
                    Text = localizationService.GetLocalizedValue(condition.TextAr, condition.TextEn),
                    CreatedDate = condition.CreatedDate,
                    LastModifiedDate = condition.UpdatedDate
                }).ToList(),
            Skills = job.JobSkills is null ? [] : mapper.Map<List<JobSkillResponseDto>>(job.JobSkills),
            Responsibilities = job.JobResponsibilities is null
                ? []
                : job.JobResponsibilities.Select(responsibility => new CandidateJobResponsibilityDto
                {
                    Id = responsibility.Id,
                    JobId = responsibility.JobId,
                    Text = localizationService.GetLocalizedValue(responsibility.TextAr, responsibility.TextEn),
                    CreatedDate = responsibility.CreatedDate,
                    LastModifiedDate = responsibility.UpdatedDate
                }).ToList(),
            RequiredAttachments = job.JobRequiredAttachments is null
                ? []
                : job.JobRequiredAttachments.Select(attachment => new CandidateJobRequiredAttachmentDto
                {
                    Id = attachment.Id,
                    JobId = attachment.JobId,
                    Title = localizationService.GetLocalizedValue(attachment.TitleAr, attachment.TitleEn),
                    IsMandatory = attachment.IsMandatory,
                    CreatedDate = attachment.CreatedDate,
                    LastModifiedDate = attachment.UpdatedDate
                }).ToList()
        };

        return Result.Ok(jobDto);
    }
}
