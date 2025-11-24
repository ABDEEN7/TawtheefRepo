using Mapster;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Mappers;

public class JobProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateJobDto, Job>()
            .Map(dest => dest, src => src.Basics.Adapt<JobBasics>())
            .Map(dest => dest.Quota, src => src.Quotas.Adapt<JobQuota>())
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Benefits, src => src.Benefits)
            .Map(dest => dest.Skills, src => src.Skills)
            .Map(dest => dest.Conditions, src => src.Conditions)
            .Map(dest => dest.Degrees, src => src.DegreeIds)
            .AfterMapping((_, dest) =>
            {
                foreach (var skill in dest.Skills)
                {
                    skill.Id = Guid.NewGuid();
                    skill.JobId = dest.Id;
                }

                foreach (var condition in dest.Conditions)
                {
                    condition.Id = Guid.NewGuid();
                    condition.JobId = dest.Id;
                }

                foreach (var degree in dest.Degrees)
                {
                    degree.Id = Guid.NewGuid();
                    degree.JobId = dest.Id;
                }


                if (dest.Quota?.ResidentsBreakdowns != null)
                {
                    foreach (var breakdown in dest.Quota.ResidentsBreakdowns)
                    {
                        breakdown.Id = Guid.NewGuid();
                        breakdown.JobQuotaId = dest.Quota.Id;
                    }
                }
            });

        config.NewConfig<JobBasicsDto, JobBasics>()
            .Map(dest => dest.RequestingDepartmentId, src => src.RequestingDeptId)
            .Map(dest => dest.JobCategoryId, src => src.JobCategoryId)
            .Map(dest => dest.GenderId, src => src.GenderId)
            .Map(dest => dest.TargetEntityId, src => src.WorkLocationId)
            .Map(dest => dest.MajorId, src => src.MajorId)
            .Map(dest => dest.WorkTypeId, src => src.WorkTypeId)
            .Map(dest => dest.Vacancies, src => src.Vacancies)
            .Map(dest => dest.Deadline, src => src.Deadline)
            .Map(dest => dest.Title, src => src.Title);

        config.NewConfig<JobQuotasDto, JobQuota>()
            .Map(dest => dest.QatariCitizens, src => src.QatariCitizens)
            .Map(dest => dest.QatarMother, src => src.QatarMother)
            .Map(dest => dest.NonQatariSpouse, src => src.NonQatariSpouse)
            .Map(dest => dest.Gcc, src => src.Gcc)
            .Map(dest => dest.QuGrads, src => src.QuGrads)
            .Map(dest => dest.Residents, src => src.Residents)
            .Map(dest => dest.ResidentsBreakdowns, src => src.ResidentsBreakdown.Adapt<List<ResidentBreakdown>>());

        config.NewConfig<ResidentBreakdownDto, ResidentBreakdown>()
            .Map(dest => dest.NationalityId, src => src.NationalityId)
            .Map(dest => dest.Percentage, src => src.Percentage);

        config.NewConfig<string, JobSkill>()
            .MapWith(src => new JobSkill 
            { 
                Id = Guid.NewGuid(),
                Text = src,
                Order = 0
            });

        config.NewConfig<string, JobCondition>()
            .MapWith(src => new JobCondition 
            { 
                Id = Guid.NewGuid(),
                Text = src,
                Order = 0
            });

        config.NewConfig<Guid, JobDegree>()
            .MapWith(src => new JobDegree 
            { 
                Id = Guid.NewGuid(),
                DegreeId = src
            });

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
            .Map(dest => dest.Degrees, src => src.Degrees.Select(d => d.Degree.Adapt<DropdownOptions>())); // ✅ Map Degree lookup to DropdownOptions

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
