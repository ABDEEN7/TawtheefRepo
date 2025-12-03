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
                .Map(dest => dest.StatusId, _ => JobStatusIds.Draft)
                .AfterMapping((src, dest) =>
                {
                    var jobId = dest.Id;

                    dest.Degrees = [.. src.Degrees.Select(degree => new JobDegree
                    {
                        Id = Guid.NewGuid(),
                        DegreeId = degree.DegreeId,
                        JobId = jobId
                    })];

                    dest.Conditions = [.. src.Conditions.Select(cond => new JobCondition
                    {
                        Id = Guid.NewGuid(),
                        JobId = jobId,
                        Text = cond.Text,
                    })];

                    dest.Skills = [.. src.Skills.Select(skill => new JobSkill
                    {
                        Id = Guid.NewGuid(),
                        JobId = jobId, 
                        ShowToApplicants = skill.ShowToApplicants,
                        SkillId = skill.SkillId
                    })];

                    dest.Responsibilities = [.. src.Responsibilities.Select(resp => new JobResponsibility
                    {
                        Id = Guid.NewGuid(),
                        JobId = jobId,
                        Text = resp.Text,
                    })];

                    dest.RequiredAttachments = src.RequiredAttachments?.Select(a =>
                        new JobRequiredAttachment
                        {
                            Id = Guid.NewGuid(),
                            JobId = jobId,
                            Title = a.Title,
                            IsMandatory = a.IsMandatory
                        }).ToList() ?? [];

                    dest.Quota = MapQuota(src.Quota, jobId);
                });

            config.NewConfig<UpdateJobDto, Job>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.StatusId)
                .Ignore(dest => dest.Invitations)
                .AfterMapping((src, dest) =>
                {
                    if (dest.Quota == null)
                    {
                        dest.Quota = MapQuota(src.Quota, dest.Id);
                    }
                    else
                    {
                        dest.Quota.QatariCitizens = src.Quota.QatariCitizens;
                        dest.Quota.QatarMother = src.Quota.QatarMother;
                        dest.Quota.NonQatariSpouse = src.Quota.NonQatariSpouse;
                        dest.Quota.Gcc = src.Quota.Gcc;
                        dest.Quota.QuGrads = src.Quota.QuGrads;
                        dest.Quota.Residents = src.Quota.Residents;

                        var quotaId = dest.Quota.Id;
                        dest.Quota.ResidentsBreakdowns = src.Quota.ResidentsBreakdowns
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
                .Map(dest => dest.Department, src => src.RequestingDepartment)
                .Map(dest => dest.Sector, src => src.Sector)
                .Map(dest => dest.Management, src => src.Management)
                .Map(dest => dest.JobCategory, src => src.JobCategory)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.WorkLocation, src => src.WorkLocation)
                .Map(dest => dest.Major, src => src.Major)
                .Map(dest => dest.SubMajor, src => src.SubMajor)
                .Map(dest => dest.WorkType, src => src.WorkType)
                .Map(dest => dest.Status, src => src.Status)
                .Map(dest => dest.Quota, src => src.Quota)
                .Map(dest => dest.Degrees, src => src.Degrees.Select(d => d.Degree))
                .Map(dest => dest.Conditions, src => src.Conditions)
                .Map(dest => dest.Skills, src => src.Skills)
                .Map(dest => dest.Responsibilities, src => src.Responsibilities)
                .Map(dest => dest.RequiredAttachments, src => src.RequiredAttachments);

            config.NewConfig<JobQuota, JobQuotaResponseDto>()
                .Map(dest => dest.ResidentsBreakdowns, src => src.ResidentsBreakdowns);

            config.NewConfig<ResidentBreakdown, ResidentBreakdownResponseDto>()
                .Map(dest => dest.Nationality, src => src.Nationality);
        }

        private static JobQuota MapQuota(JobQuotaDto quotaDto, Guid jobId)
        {
            var quotaId = Guid.NewGuid();

            return new JobQuota
            {
                Id = quotaId,
                JobId = jobId,
                QatariCitizens = quotaDto.QatariCitizens,
                QatarMother = quotaDto.QatarMother,
                NonQatariSpouse = quotaDto.NonQatariSpouse,
                Gcc = quotaDto.Gcc,
                QuGrads = quotaDto.QuGrads,
                Residents = quotaDto.Residents,
                ResidentsBreakdowns = [.. quotaDto.ResidentsBreakdowns
                    .Select(rb => new ResidentBreakdown
                    {
                        Id = Guid.NewGuid(),
                        JobQuotaId = quotaId,
                        NationalityId = rb.NationalityId,
                        Percentage = rb.Percentage
                    })]
            };
        }
    }
}
