using Mapster;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Mappers;

public class JobProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Create Job from CreateJobDto
        config.NewConfig<CreateJobDto, Job>()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.StatusId, _ => JobStatusIds.Draft)
            .Map(dest => dest.QuotaId, _ => Guid.NewGuid())
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.RequestingDepartmentId, src => src.RequestingDepartmentId)
            .Map(dest => dest.JobCategoryId, src => src.JobCategoryId)
            .Map(dest => dest.GenderId, src => src.GenderId)
            .Map(dest => dest.WorkLocationId, src => src.WorkLocationId)
            .Map(dest => dest.MajorId, src => src.MajorId)
            .Map(dest => dest.WorkTypeId, src => src.WorkTypeId)
            .Map(dest => dest.Vacancies, src => src.Vacancies)
            .Map(dest => dest.Deadline, src => src.Deadline)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Benefits, src => src.Benefits)
            .Ignore(dest => dest.Quota!)
            .Ignore(dest => dest.Degrees)
            .Ignore(dest => dest.Conditions)
            .Ignore(dest => dest.Skills);

        // FIXED: Map ResidentsBreakdown (DTO) to ResidentsBreakdowns (Entity)
        config.NewConfig<(CreateJobDto dto, Guid quotaId), JobQuota>()
            .Map(dest => dest.Id, src => src.quotaId)
            .Map(dest => dest.QatariCitizens, src => src.dto.Quota.QatariCitizens)
            .Map(dest => dest.QatarMother, src => src.dto.Quota.QatarMother)
            .Map(dest => dest.NonQatariSpouse, src => src.dto.Quota.NonQatariSpouse)
            .Map(dest => dest.Gcc, src => src.dto.Quota.Gcc)
            .Map(dest => dest.QuGrads, src => src.dto.Quota.QuGrads)
            .Map(dest => dest.Residents, src => src.dto.Quota.Residents)
            .Map(dest => dest.ResidentsBreakdowns, src => src.dto.Quota.ResidentsBreakdown) // FIXED: Map to correct property name
            .AfterMapping((_, dest) =>
            {
                // Set IDs for resident breakdowns
                foreach (var breakdown in dest.ResidentsBreakdowns)
                {
                    breakdown.Id = Guid.NewGuid();
                    breakdown.JobQuotaId = dest.Id;
                }
            });

        // Map ResidentBreakdownDto to ResidentBreakdown
        config.NewConfig<ResidentBreakdownDto, ResidentBreakdown>()
            .Map(dest => dest.NationalityId, src => src.NationalityId)
            .Map(dest => dest.Percentage, src => src.Percentage)
            .Ignore(dest => dest.Id) // Will be set in AfterMapping
            .Ignore(dest => dest.JobQuotaId); // Will be set in AfterMapping

        // Create JobDegree from DegreeIds
        config.NewConfig<(Guid jobId, List<Guid> degreeIds), List<JobDegree>>()
            .MapWith(src => src.degreeIds.Select(degreeId => new JobDegree
            {
                Id = Guid.NewGuid(),
                JobId = src.jobId,
                DegreeId = degreeId
            }).ToList());

        // Create JobCondition from conditions
        config.NewConfig<(Guid jobId, List<string> conditions), List<JobCondition>>()
            .MapWith(src => src.conditions.Select((condition, index) => new JobCondition
            {
                Id = Guid.NewGuid(),
                JobId = src.jobId,
                Text = condition,
                Order = index
            }).ToList());

        // Create JobSkill from skills
        config.NewConfig<(Guid jobId, List<string> skills), List<JobSkill>>()
            .MapWith(src => src.skills.Select((skill, index) => new JobSkill
            {
                Id = Guid.NewGuid(),
                JobId = src.jobId,
                Text = skill,
                Order = index
            }).ToList());

        // Response mappings
        config.NewConfig<Job, JobResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Basics, src => new JobBasicsResponseDto
            {
                Title = src.Title,
                Vacancies = src.Vacancies,
                Deadline = src.Deadline,
                Department = src.RequestingDepartment.Adapt<DropdownOptions>(),
                JobCategory = src.JobCategory.Adapt<DropdownOptions>(),
                Major = src.Major.Adapt<DropdownOptions>(),
                WorkType = src.WorkType.Adapt<DropdownOptions>(),
                Gender = src.Gender.Adapt<DropdownOptions>(),
                TargetEntity = src.WorkLocation.Adapt<DropdownOptions>()
            })
            .Map(dest => dest.Quotas, src => src.Quota.Adapt<JobQuotasResponseDto>())
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Benefits, src => src.Benefits)
            .Map(dest => dest.Status, src => src.Status.Adapt<DropdownOptions>())
            .Map(dest => dest.Skills, src => src.Skills.OrderBy(s => s.Order).Select(s => s.Text))
            .Map(dest => dest.Conditions, src => src.Conditions.OrderBy(c => c.Order).Select(c => c.Text))
            .Map(dest => dest.Degrees, src => src.Degrees.Select(d => d.Degree.Adapt<DropdownOptions>()));

        config.NewConfig<JobQuota, JobQuotasResponseDto>()
            .Map(dest => dest.QatariCitizens, src => src.QatariCitizens)
            .Map(dest => dest.QatarMother, src => src.QatarMother)
            .Map(dest => dest.NonQatariSpouse, src => src.NonQatariSpouse)
            .Map(dest => dest.Gcc, src => src.Gcc)
            .Map(dest => dest.QuGrads, src => src.QuGrads)
            .Map(dest => dest.Residents, src => src.Residents)
            .Map(dest => dest.ResidentsBreakdown, src => src.ResidentsBreakdowns.Adapt<List<ResidentBreakdownResponseDto>>());

        config.NewConfig<ResidentBreakdown, ResidentBreakdownResponseDto>()
            .Map(dest => dest.Nationality, src => src.Nationality.Adapt<DropdownOptions>())
            .Map(dest => dest.Percentage, src => src.Percentage);
    }
}
