using Mapster;
using Tawtheef.Application.Features.Operations.Employee.Job.DTOs;
using Tawtheef.Application.Features.Operations.Employee.Job.Dtos;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Common.Mappers
{
    public class JobProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateJobDto, Job>()
                .Map(dest => dest.Id, _ => Guid.NewGuid())
                .Map(dest => dest.JobStatusId, _ => JobStatusIds.Draft)
                .AfterMapping((src, dest) =>
                {
                    var jobId = dest.Id;

                    dest.JobDegrees = src.Degrees?.Select(degree => new JobDegree
                    {
                        Id = Guid.NewGuid(),
                        DegreeId = degree.DegreeId,
                        JobId = jobId
                    }).ToList() ?? [];

                    dest.JobConditions = src.Conditions?.Select(cond => new JobCondition
                    {
                        Id = Guid.NewGuid(),
                        JobId = jobId,
                        TextAr = cond.TextAr,
                        TextEn = cond.TextEn,
                    }).ToList() ?? [];

                    dest.JobSkills = src.Skills?.Select(skill => new JobSkill
                    {
                        Id = Guid.NewGuid(),
                        JobId = jobId,
                        ShowToApplicants = skill.ShowToApplicants,
                        SkillId = skill.SkillId
                    }).ToList() ?? [];

                    dest.JobResponsibilities = src.Responsibilities!.Select(resp => new JobResponsibility
                    {
                        Id = Guid.NewGuid(),
                        JobId = jobId,
                        TitleAr = resp.TextAr,
                        TitleEn = resp.TextEn,
                    }).ToList() ?? [];

                    dest.JobRequiredAttachments = src.RequiredAttachments?.Select(a =>
                        new JobRequiredAttachment
                        {
                            Id = Guid.NewGuid(),
                            JobId = jobId,
                            TitleAr = a.TitleAr,
                            TitleEn = a.TitleEn,
                            IsMandatory = a.IsMandatory
                        }).ToList() ?? [];

                    dest.JobQuota = src.Quota != null
                        ? MapQuota(src.Quota, jobId)
                        : null;
                });


            config.NewConfig<UpdateJobDto, Job>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.JobStatusId)
                .Ignore(dest => dest.Invitations)
                .AfterMapping((src, dest) =>
                {
                    if (dest.JobQuota == null)
                    {
                        dest.JobQuota = MapQuota(src.Quota, dest.Id);
                    }
                    else
                    {
                        dest.JobQuota.QatariCitizens = src.Quota!.QatariCitizens;
                        dest.JobQuota.QatarMother = src.Quota.QatarMother;
                        dest.JobQuota.NonQatariSpouse = src.Quota.NonQatariSpouse;
                        dest.JobQuota.Gcc = src.Quota.Gcc;
                        dest.JobQuota.QuGrads = src.Quota.QuGrads;
                        dest.JobQuota.Residents = src.Quota.Residents;

                        var quotaId = dest.JobQuota.Id;
                        dest.JobQuota.ResidentsBreakdowns = src.Quota.ResidentsBreakdowns!
                            .Select(rb => new ResidentBreakdown
                            {
                                Id = Guid.NewGuid(),
                                JobQuotaId = quotaId,
                                NationalityId = rb.NationalityId,
                                Percentage = rb.Percentage
                            }).ToList();
                    }
                });


            config.NewConfig<Job, JobResponseDto>()
                .Map(dest => dest.Sector, src => src.Sector)
                .Map(dest => dest.Management, src => src.Management)
                .Map(dest => dest.Department, src => src.Department)
                .Map(dest => dest.JobCategory, src => src.JobCategory)
                .Map(dest => dest.WorkLocation, src => src.WorkLocation)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Major, src => src.Major)
                .Map(dest => dest.SubMajor, src => src.SubMajor)
                .Map(dest => dest.WorkType, src => src.WorkType)
                .Map(dest => dest.Status, src => src.JobStatus)
                .Map(dest => dest.Quota, src => src.JobQuota)
                .Map(dest => dest.Degrees, src => src.JobDegrees)
                .Map(dest => dest.Conditions, src => src.JobConditions)
                .Map(dest => dest.Skills, src => src.JobSkills)
                .Map(dest => dest.Responsibilities, src => src.JobResponsibilities)
                .Map(dest => dest.RequiredAttachments, src => src.JobRequiredAttachments)
                .TwoWays();


            config.NewConfig<JobQuota, JobQuotaResponseDto>()
                .Map(dest => dest.ResidentsBreakdowns, src => src.ResidentsBreakdowns)
                .TwoWays();

            config.NewConfig<ResidentBreakdown, ResidentBreakdownResponseDto>()
                .Map(dest => dest.Nationality, src => src.Nationality)
                .TwoWays();

            config.NewConfig<JobDegree, JobDegreeResponseDto>()
                .Map(dest => dest.Degree, src => src.Degree)
                .TwoWays();

            config.NewConfig<JobCondition, JobConditionResponseDto>()
                .TwoWays();

            config.NewConfig<JobSkill, JobSkillResponseDto>()
                .Map(dest => dest.Skill, src => src.Skill)
                .TwoWays();

            config.NewConfig<JobResponsibility, JobResponsibilityResponseDto>()
                .TwoWays();

            config.NewConfig<JobRequiredAttachment, JobRequiredAttachmentResponseDto>()
                .TwoWays();

            config.NewConfig<List<Job>, List<JobResponseDto>>().TwoWays();
        }

        private static JobQuota MapQuota(JobQuotaRequestDto? quotaDto, Guid jobId)
        {
            var quotaId = Guid.NewGuid();

            return new JobQuota
            {
                Id = quotaId,
                JobId = jobId,
                QatariCitizens = quotaDto!.QatariCitizens,
                QatarMother = quotaDto.QatarMother,
                NonQatariSpouse = quotaDto.NonQatariSpouse,
                Gcc = quotaDto.Gcc,
                QuGrads = quotaDto.QuGrads,
                Residents = quotaDto.Residents,
                ResidentsBreakdowns = quotaDto.ResidentsBreakdowns!
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
    }
}
