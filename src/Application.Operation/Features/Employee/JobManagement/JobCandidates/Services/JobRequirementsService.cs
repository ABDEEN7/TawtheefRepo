using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobSpecialization = Application.Operation.Features.Employee.JobManagement.JobCandidates.Models.JobSpecialization;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Services;

public class JobRequirementsService(IUnitOfWork unitOfWork) : IJobRequirementsService
{
    public async Task<JobRequirements> GetAsync(Tawtheef.Domain.Entities.Recruitment.Job job)
    {
        var jobSpecializations = job.JobSpecializations?
            .Select(js => new JobSpecialization(js.MajorId, js.SubMajorId))
            .ToList() ?? [];

        var majorIds = new[]
            {
                job.MajorId,
                job.SubMajorId
            }
            .Where(id => id.HasValue && id.Value != Guid.Empty)
            .Select(id => id!.Value)
            .Concat(jobSpecializations.SelectMany(js => new[] { js.MajorId, js.SubMajorId }))
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        var qualificationLevelIds = job.JobDegrees
            .Select(jd => jd.DegreeId)
            .ToList();

        var requiredSkillIds = await unitOfWork.GetEntityRepository<JobSkill>().DbSet
            .AsNoTracking()
            .Where(js => js.JobId == job.Id && js.IsRequired)
            .Select(js => js.SkillId)
            .Distinct()
            .ToListAsync();

        return new JobRequirements(
            job.MajorId,
            job.SubMajorId,
            jobSpecializations,
            qualificationLevelIds,
            requiredSkillIds);
    }
}
