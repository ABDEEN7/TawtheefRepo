using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

using Application.Operation.Common.Validations;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class UpdateJobSkillsCommandHandler(
    IJobValidationService validationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobSkillsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobSkillsCommand request, CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<JobEntity>().DbSet
            .Include(j => j.JobSkills)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        
        if (job is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        var validationResult = await validationService.ValidateSkillsUpdate(request.Data, job);
        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.First().ErrorMessage);
        
        // 4. Sync skills collection
        await SyncSkills(unitOfWork, request.JobId, job.JobSkills, request.Data.Skills, cancellationToken);

        // 5. Persist
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }

    private async Task SyncSkills(IUnitOfWork uow,Guid jobId, ICollection<JobSkill> existsSkills, ICollection<JobSkillRequestDto> newSkills, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<JobSkill>();
        // Deduplicate input by SkillId (keep first occurrence)
        var distinctNew = newSkills
            .GroupBy(s => s.SkillId)
            .Select(g => g.First())
            .ToList();

        var desiredIds = distinctNew.Select(s => s.SkillId).ToHashSet();
        var existingIds = existsSkills.Select(s => s.SkillId).ToHashSet();

        // Remove items not in the new set
        var toRemove = existsSkills.Where(s => !desiredIds.Contains(s.SkillId)).ToList();
        foreach (var item in toRemove)
            await repo.DeleteAsync(item.Id,ct);

        // Update ShowToApplicants for existing items
        var newLookup = distinctNew.ToDictionary(s => s.SkillId);
        foreach (var existing in existsSkills)
        {
            if (newLookup.TryGetValue(existing.SkillId, out var dto))
                existing.ShowToApplicants = dto.ShowToApplicants;
        }

        // Add new items
        foreach (var dto in distinctNew.Where(s => !existingIds.Contains(s.SkillId)))
        {
            await repo.AddAsync(new JobSkill
            {
                JobId = jobId,
                SkillId = dto.SkillId,
                ShowToApplicants = dto.ShowToApplicants
            },ct);
        }
    }
}
