using Application.Operation.Features.Employee.JobManagement.JobOperations.Queries;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.JobManagement.JobOperations.Handlers.Queries;

public sealed class CheckJobInvitationsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<CheckJobInvitationsQuery, IResult<bool>>
{
    public async Task<IResult<bool>> Handle(CheckJobInvitationsQuery request, CancellationToken cancellationToken)
    {
        var hasInvitations = await uow.GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .AnyAsync(x => x.JobId == request.JobId, cancellationToken);

        return Result.Ok(hasInvitations);
    }
}
