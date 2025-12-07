using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Mappers;

public static class JobManualMapper
{
    public static JobResponseDto Map(Job job)
    {
        return new JobResponseDto
        {
            Id = job.Id,
            TitleAr = job.TitleAr,
            TitleEn = job.TitleEn,
            NumberOfVacancies = job.NumberOfVacancies,
            MinimumAge = job.MinimumAge,
            MaximumAge = job.MaximumAge,
            YearsOfExperience = job.YearsOfExperience,
            ClosingDate = job.ClosingDate,
            CreatedBy = job.CreatedBy,
            CreatedDate = job.CreatedDate,
            ModifiedBy  = job.UpdatedBy,
            ModifiedDate = job.UpdatedDate,

            OverViewAr = job.OverViewAr,
            OverViewEn = job.OverViewEn,

            QualificationDescriptionAr = job.QualificationDescriptionAr,
            QualificationDescriptionEn = job.QualificationDescriptionEn,

            BenefitsAr = job.BenefitsAr,
            BenefitsEn = job.BenefitsEn,

            Sector = MapDropdown(job.Sector),
            Management = MapDropdown(job.Management),
            Department = MapDropdown(job.Department),
            JobCategory = MapDropdown(job.JobCategory),
            WorkLocation = MapDropdown(job.WorkLocation),
            Gender = MapDropdown(job.Gender),
            Major = MapDropdown(job.Major),
            SubMajor = MapDropdown(job.SubMajor),
            WorkType = MapDropdown(job.WorkType),
            Status = MapDropdown(job.JobStatus),

            Degrees = job.JobDegrees?.Select(MapDegree).ToList() ?? [],
            Conditions = job.JobConditions?.Select(MapCondition).ToList() ?? [],
            Skills = job.JobSkills?.Select(MapSkill).ToList() ?? [],
            Responsibilities = job.JobResponsibilities?.Select(MapResponsibility).ToList() ?? [],
            RequiredAttachments = job.JobRequiredAttachments?.Select(MapAttachment).ToList() ?? [],

            Quota = job.JobQuota != null ? MapQuota(job.JobQuota) : null
        };
    }

    private static DropdownOptions? MapDropdown(LookupBase? lookup)
    {
        if (lookup == null) return null;

        return new DropdownOptions
        {
            Id = lookup.Id,
            BackendName = lookup.BackendName,
            Name = lookup.GetLocalizedName("ar") ?? string.Empty,
            Description = lookup.GetLocalizedDescription("ar"),
        };
    }

    private static JobDegreeResponseDto MapDegree(JobDegree degree)
        => new()
        {
            DegreeId = degree.DegreeId,
            Degree = MapDropdown(degree.Degree)
        };

    private static JobConditionResponseDto MapCondition(JobCondition cond)
        => new()
        {
            Id = cond.Id,
            TextAr = cond.TextAr,
            TextEn = cond.TextEn
        };

    private static JobSkillResponseDto MapSkill(JobSkill skill)
        => new()
        {
            Id = skill.Id,
            SkillId = skill.SkillId,
            ShowToApplicants = skill.ShowToApplicants,
            Skill = MapDropdown(skill.Skill)
        };

    private static JobResponsibilityResponseDto MapResponsibility(JobResponsibility resp)
        => new()
        {
            Id = resp.Id,
            TextAr = resp.TitleAr,
            TextEn = resp.TitleEn
        };

    private static JobRequiredAttachmentResponseDto MapAttachment(JobRequiredAttachment att)
        => new()
        {
            Id = att.Id,
            TitleAr = att.TitleAr,
            TitleEn = att.TitleEn,
            IsMandatory = att.IsMandatory
        };

    private static JobQuotaResponseDto MapQuota(JobQuota q)
        => new()
        {
            QatariCitizens = q.QatariCitizens,
            QatarMother = q.QatarMother,
            NonQatariSpouse = q.NonQatariSpouse,
            Gcc = q.Gcc,
            QuGrads = q.QuGrads,
            Residents = q.Residents,
            ResidentsBreakdowns = q.ResidentsBreakdowns
                ?.Select(MapResidentBreakdown)
                .ToList() ?? []
        };

    private static ResidentBreakdownResponseDto MapResidentBreakdown(ResidentBreakdown rb)
        => new()
        {
            NationalityId = rb.NationalityId,
            Percentage = rb.Percentage,
            Nationality = MapDropdown(rb.Nationality)
        };
}
