using Application.Operation.Features.Admin.JobTitles.Commands;
using Application.Operation.Features.Admin.JobTitles.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.JobTitles.Handlers.Commands;

public sealed class InsertJobTitleCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<InsertJobTitleCommand, IResult<JobTitleAdminDto>>
{
    public async Task<IResult<JobTitleAdminDto>> Handle(
        InsertJobTitleCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<JobTitle>();

        var normalizedJobNumber = request.JobNumber.Trim();
        var normalizedJobNameAr = request.JobNameAr.Trim();
        var normalizedJobNameEn = request.JobNameEn.Trim();

        if (string.IsNullOrWhiteSpace(normalizedJobNumber) ||
            string.IsNullOrWhiteSpace(normalizedJobNameAr) ||
            string.IsNullOrWhiteSpace(normalizedJobNameEn))
            return Result.Fail<JobTitleAdminDto>(ErrorsCodes.InvalidName);

        var existingJobTitle = await repository.DbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.JobNumber == normalizedJobNumber, cancellationToken);

        if (existingJobTitle is { IsDeleted: false })
            return Result.Fail<JobTitleAdminDto>(ErrorsCodes.JobTitleNumberExists);

        if (existingJobTitle is { IsDeleted: true })
        {
            existingJobTitle.JobNameAr = normalizedJobNameAr;
            existingJobTitle.JobNameEn = normalizedJobNameEn;
            existingJobTitle.IsActive = true;
            existingJobTitle.IsDeleted = false;
            existingJobTitle.DeletedById = null;
            existingJobTitle.DeletedDate = null;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(ToDto(existingJobTitle));
        }

        var newEntity = new JobTitle
        {
            Id = Guid.NewGuid(),
            JobNumber = normalizedJobNumber,
            JobNameAr = normalizedJobNameAr,
            JobNameEn = normalizedJobNameEn,
            IsActive = true
        };

        await repository.AddAsync(newEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(ToDto(newEntity));
    }

    private static JobTitleAdminDto ToDto(JobTitle entity)
        => new(entity.Id, entity.JobNumber, entity.JobNameAr, entity.JobNameEn, entity.IsActive);
}
