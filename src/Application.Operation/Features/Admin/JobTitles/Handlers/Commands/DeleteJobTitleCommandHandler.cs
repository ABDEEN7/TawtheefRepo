using Application.Operation.Features.Admin.JobTitles.Commands;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Features.Admin.JobTitles.Handlers.Commands;

public sealed class DeleteJobTitleCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteJobTitleCommand, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(DeleteJobTitleCommand request, CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<JobTitle>();

        var entity = await repository.DbSet.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity is null)
            return Result.Fail<bool>(ErrorsCodes.NotFound);

        var isUsed = await unitOfWork
            .GetEntityRepository<Tawtheef.Domain.Entities.Recruitment.Job>()
            .DbSet
            .AnyAsync(x => x.JobTitleId == request.Id, cancellationToken);

        if (isUsed)
            return Result.Fail<bool>(ErrorsCodes.JobTitleInUse);

        await repository.DeleteAsync(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(true);
    }
}
