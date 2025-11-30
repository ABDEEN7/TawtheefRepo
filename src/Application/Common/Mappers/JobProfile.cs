using Mapster;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Tawtheef.Application.Common.Mappers;

public class JobProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        ConfigureCreateMappings(config);
        ConfigureUpdateMappings(config);
        ConfigureResponseMappings(config);
    }

    private static void ConfigureCreateMappings(TypeAdapterConfig config)
    {
        // ============================================
        // CREATE JOB - Complete mapping with all relationships
        // ============================================
        config.NewConfig<CreateJobDto, Job>()
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.StatusId, _ => JobStatusIds.Draft)
            .Map(dest => dest.QuotaId, _ => Guid.NewGuid())
            // Don't auto-map collections - we'll handle in AfterMapping
            .Ignore(dest => dest.Skills)
            .Ignore(dest => dest.Conditions)
            .Ignore(dest => dest.Degrees)
            .Ignore(dest => dest.Quota!)
            .AfterMapping((src, dest) =>
            {
                // Map Quota
                dest.Quota = MapQuota(src.Quota, dest.QuotaId);
                
                // Map Skills
                dest.Skills = src.Skills
                    .Select((text, index) => new JobSkill
                    {
                        Id = Guid.NewGuid(),
                        JobId = dest.Id,
                        Text = text,
                        Order = index + 1
                    })
                    .ToList();

                // Map Conditions
                dest.Conditions = src.Conditions
                    .Select((text, index) => new JobCondition
                    {
                        Id = Guid.NewGuid(),
                        JobId = dest.Id,
                        Text = text,
                        Order = index + 1
                    })
                    .ToList();

                // Map Degrees
                dest.Degrees = src.DegreeIds
                    .Select(degreeId => new JobDegree
                    {
                        Id = Guid.NewGuid(),
                        JobId = dest.Id,
                        DegreeId = degreeId
                    })
                    .ToList();
            });
    }

    private static JobQuota MapQuota(JobQuotaDto dto, Guid quotaId)
    {
        return new JobQuota
        {
            Id = quotaId,
            QatariCitizens = dto.QatariCitizens,
            QatarMother = dto.QatarMother,
            NonQatariSpouse = dto.NonQatariSpouse,
            Gcc = dto.Gcc,
            QuGrads = dto.QuGrads,
            Residents = dto.Residents,
            ResidentsBreakdowns = dto.ResidentsBreakdowns
                .Select(rb => new ResidentBreakdown
                {
                    Id = Guid.NewGuid(),
                    JobQuotaId = quotaId,
                    NationalityId = rb.NationalityId,
                    Percentage = rb.Percentage
                })
                .ToList()
        };
    }

    private static void ConfigureUpdateMappings(TypeAdapterConfig config)
    {
        // ============================================
        // UPDATE JOB - Map DTO to existing entity
        // ============================================
        config.NewConfig<UpdateJobDto, Job>()
            .Map(dest => dest.StatusId, _ => JobStatusIds.Draft)
            // Collections will be handled manually in the handler
            .Ignore(dest => dest.Skills)
            .Ignore(dest => dest.Conditions)
            .Ignore(dest => dest.Degrees)
            .Ignore(dest => dest.Quota!)
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.QuotaId);
    }

    private static void ConfigureResponseMappings(TypeAdapterConfig config)
    {
        // ============================================
        // JOB RESPONSE
        // ============================================
        config.NewConfig<Job, JobResponseDto>()
            .Map(dest => dest.RequestingDepartment, src => src.RequestingDepartment)
            .Map(dest => dest.JobCategory, src => src.JobCategory)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.WorkLocation, src => src.WorkLocation)
            .Map(dest => dest.Major, src => src.Major)
            .Map(dest => dest.WorkType, src => src.WorkType)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Quota, src => src.Quota)
            .Map(dest => dest.Skills, src => src.Skills.OrderBy(s => s.Order).Select(s => s.Text))
            .Map(dest => dest.Conditions, src => src.Conditions.OrderBy(c => c.Order).Select(c => c.Text))
            .Map(dest => dest.Degrees, src => src.Degrees.Select(d => d.Degree));

        // ============================================
        // JOB QUOTA RESPONSE
        // ============================================
        config.NewConfig<JobQuota, JobQuotasResponseDto>()
            .Map(dest => dest.ResidentsBreakdowns, src => src.ResidentsBreakdowns);

        // ============================================
        // RESIDENT BREAKDOWN RESPONSE
        // ============================================
        config.NewConfig<ResidentBreakdown, ResidentBreakdownResponseDto>()
            .Map(dest => dest.Nationality, src => src.Nationality);
    }
}
