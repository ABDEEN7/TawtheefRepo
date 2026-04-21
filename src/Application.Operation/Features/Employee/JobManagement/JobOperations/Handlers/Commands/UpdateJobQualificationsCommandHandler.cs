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

public class UpdateJobQualificationsCommandHandler(
    IJobValidationService validationService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJobQualificationsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(UpdateJobQualificationsCommand request, CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<JobEntity>().DbSet
            .Include(j => j.JobDegrees)
            .Include(j => j.JobSpecializations)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        
        if (job is null)
            return Result.Fail<Unit>(ErrorsCodes.NotFound);

        var validationResult = await validationService.ValidateQualificationsUpdate(request.Data, job);
        if (!validationResult.IsValid)
            return Result.Fail<Unit>(validationResult.Errors.First().ErrorMessage);

        // 4. Update scalar qualification fields
        job.MajorId = request.Data.MajorId;
        job.SubMajorId = request.Data.SubMajorId;
        job.QualificationDescriptionAr = request.Data.QualificationsDescriptionAr;
        job.QualificationDescriptionEn = request.Data.QualificationsDescriptionEn;

        // 5. Sync degrees collection
        if (request.Data.Degrees != null)
            await SyncDegrees(unitOfWork, job.Id, job.JobDegrees, request.Data.Degrees.Select(d => d.DegreeId).ToList(), cancellationToken);

        // 6. Sync specializations collection
        if (request.Data.JobSpecializations != null)
            await SyncSpecializations(unitOfWork, job.Id, job.JobSpecializations, request.Data.JobSpecializations, cancellationToken);

        // 7. Persist
               await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
    private async Task SyncDegrees(IUnitOfWork uow,Guid jobId, ICollection<JobDegree> existsDegrees, ICollection<Guid> newDegreeIds, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<JobDegree>();
        // Deduplicate
        var desiredIds = newDegreeIds.Distinct().ToHashSet();
        var existingIds = existsDegrees.Select(d => d.DegreeId).ToHashSet();

        // Remove items not in the new set
        var toRemove = existsDegrees.Where(d => !desiredIds.Contains(d.DegreeId)).ToList();
        foreach (var item in toRemove)
            await repo.DeleteAsync(item.Id, ct);

        // Add new items
        foreach (var id in desiredIds.Where(id => !existingIds.Contains(id)))
        {
            await repo.AddAsync(new JobDegree
            {
                JobId = jobId,
                DegreeId = id
            }, ct);
        }
    }
    private async Task SyncSpecializations(IUnitOfWork uow,Guid jobId, ICollection<JobSpecialization> existsSpecs, ICollection<JobSpecializationRequestDto> newSpecs, CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<JobSpecialization>();
        // Deduplicate by (MajorId, SubMajorId) composite key
        var distinctNew = newSpecs
            .GroupBy(s => (s.MajorId, s.SubMajorId))
            .Select(g => g.First())
            .ToList();

        var desiredKeys = distinctNew
            .Select(s => (s.MajorId, s.SubMajorId))
            .ToHashSet();

        var existingKeys = existsSpecs
            .Select(s => (s.MajorId, s.SubMajorId))
            .ToHashSet();

        // Remove items not in the new set
        var toRemove = existsSpecs
            .Where(s => !desiredKeys.Contains((s.MajorId, s.SubMajorId)))
            .ToList();
        foreach (var item in toRemove)
            await repo.DeleteAsync(item.Id, ct);

        // Add new items
        foreach (var dto in distinctNew.Where(s => !existingKeys.Contains((s.MajorId, s.SubMajorId))))
        {
            await repo.AddAsync(new JobSpecialization
            {
                JobId = jobId,
                MajorId = dto.MajorId,
                SubMajorId = dto.SubMajorId
            }, ct);
        }
    }
}
