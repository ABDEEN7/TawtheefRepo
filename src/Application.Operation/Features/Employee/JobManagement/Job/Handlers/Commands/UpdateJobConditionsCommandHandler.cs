using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

using Application.Operation.Common.Validations;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public class UpdateJobConditionsCommandHandler(
    IJobValidationService validationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobConditionsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobConditionsCommand request, CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<JobEntity>().DbSet
            .Include(j => j.JobConditions)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        
        if (job is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        var validationResult = await validationService.ValidateConditionsUpdate(request.Data, job);
        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.First().ErrorMessage);

        // 4. Sync conditions collection
        await SyncConditions(unitOfWork, job.Id, job.JobConditions, request.Data.Conditions, cancellationToken);

        // 5. Persist
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }

    private async Task SyncConditions(IUnitOfWork uow,Guid jobId, ICollection<JobCondition> existsConditions, ICollection<JobConditionRequestDto> newConditions, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<JobCondition>();
        // Deduplicate by TextAr (case-insensitive)
        var distinctNew = newConditions
            .GroupBy(c => c.TextAr, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        var desiredKeys = distinctNew.Select(c => c.TextAr).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingKeys = existsConditions.Select(c => c.TextAr).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Remove items not in the new set
        var toRemove = existsConditions.Where(c => !desiredKeys.Contains(c.TextAr)).ToList();
        foreach (var item in toRemove)
            await repo.DeleteAsync(item.Id, ct);

        // Update TextEn for existing items
        var newLookup = distinctNew.ToDictionary(c => c.TextAr, c => c, StringComparer.OrdinalIgnoreCase);
        foreach (var existing in existsConditions)
        {
            if (newLookup.TryGetValue(existing.TextAr, out var dto))
                existing.TextEn = dto.TextEn;
        }

        // Add new items
        foreach (var dto in distinctNew.Where(c => !existingKeys.Contains(c.TextAr)))
        {
            await repo.AddAsync(new JobCondition
            {
                JobId = jobId,
                TextAr = dto.TextAr,
                TextEn = dto.TextEn
            }, ct);
        }
    }
}
