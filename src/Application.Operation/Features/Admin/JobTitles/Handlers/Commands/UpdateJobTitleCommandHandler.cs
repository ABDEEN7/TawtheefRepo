using Application.Operation.Features.Admin.JobTitles.Commands;
using Application.Operation.Features.Admin.JobTitles.DTOs;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.JobTitles.Handlers.Commands;

public sealed class UpdateJobTitleCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateJobTitleCommand, IResult<JobTitleAdminDto>>
{
    public async Task<IResult<JobTitleAdminDto>> Handle(
        UpdateJobTitleCommand request,
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

        var entity = await repository.DbSet.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            return Result.Fail<JobTitleAdminDto>(ErrorsCodes.NotFound);

        var hasConflict = await repository.DbSet.AnyAsync(
            x => x.Id != entity.Id && x.JobNumber == normalizedJobNumber,
            cancellationToken);

        if (hasConflict)
            return Result.Fail<JobTitleAdminDto>(ErrorsCodes.JobTitleNumberExists);

        entity.JobNumber = normalizedJobNumber;
        entity.JobNameAr = normalizedJobNameAr;
        entity.JobNameEn = normalizedJobNameEn;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(ToDto(entity));
    }

    private static JobTitleAdminDto ToDto(JobTitle entity)
        => new(entity.Id, entity.JobNumber, entity.JobNameAr, entity.JobNameEn, entity.IsActive);
}
