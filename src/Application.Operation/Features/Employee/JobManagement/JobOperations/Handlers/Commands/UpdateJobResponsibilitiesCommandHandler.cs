using Application.Operation.Features.Employee.JobManagement.JobOperations.Commands;
using Application.Operation.Features.Employee.JobManagement.JobOperations.DTOs;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;
using Application.Operation.Common.Validations;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Commands;

public class UpdateJobResponsibilitiesCommandHandler(
    IJobValidationService validationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobResponsibilitiesCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobResponsibilitiesCommand request, CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<JobEntity>().DbSet
            .Include(j => j.JobResponsibilities)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        
        if (job is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        var validationResult = await validationService.ValidateResponsibilitiesUpdate(request.Data, job);
        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.First().ErrorMessage);

        // 4. Sync responsibilities collection
        await SyncResponsibilities(unitOfWork, request.JobId, job.JobResponsibilities, request.Data.Responsibilities, cancellationToken);

        // 5. Persist
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }

    private async Task SyncResponsibilities(IUnitOfWork uow,Guid jobId, ICollection<JobResponsibility> existsResponsibilities, ICollection<JobResponsibilityRequestDto> newResponsibilities, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<JobResponsibility>();
        // Deduplicate by TextAr (case-insensitive)
        var distinctNew = newResponsibilities
            .GroupBy(r => r.TextAr, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();

        var desiredKeys = distinctNew.Select(r => r.TextAr).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingKeys = existsResponsibilities.Select(r => r.TextAr).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Remove items not in the new set
        var toRemove = existsResponsibilities.Where(r => !desiredKeys.Contains(r.TextAr)).ToList();
        foreach (var item in toRemove)
            await repo.DeleteAsync(item.Id, ct);

        // Update TextEn for existing items
        var newLookup = distinctNew.ToDictionary(r => r.TextAr, r => r, StringComparer.OrdinalIgnoreCase);
        foreach (var existing in existsResponsibilities)
        {
            if (newLookup.TryGetValue(existing.TextAr, out var dto))
                existing.TextEn = dto.TextEn;
        }

        // Add new items
        foreach (var dto in distinctNew.Where(r => !existingKeys.Contains(r.TextAr)))
        {
            await repo.AddAsync(new JobResponsibility
            {
                JobId = jobId,
                TextAr = dto.TextAr,
                TextEn = dto.TextEn
            },ct);
        }
    }
}
