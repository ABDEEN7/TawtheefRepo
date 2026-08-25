using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using Application.Operation.Features.Employee.Exceptions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Queries;

public sealed class GetInvitationExceptionsSummaryQueryHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider accessContextProvider)
    : IRequestHandler<GetInvitationExceptionsSummaryQuery, IResult<InvitationExceptionsSummaryDto>>
{
    public async Task<IResult<InvitationExceptionsSummaryDto>> Handle(
        GetInvitationExceptionsSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var accessibleJobs = unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess());

        var counts = await unitOfWork.GetEntityRepository<InvitationException>().DbSet
            .AsNoTracking()
            .Where(invitationException =>
                accessibleJobs.Any(job => job.Id == invitationException.JobId))
            .GroupBy(invitationException => invitationException.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        var byStatus = counts.ToDictionary(item => item.Status, item => item.Count);
        var total = counts.Sum(item => item.Count);

        return Result.Ok(new InvitationExceptionsSummaryDto(
            total,
            byStatus.GetValueOrDefault(InvitationExceptionStatus.ReadyToSend),
            byStatus.GetValueOrDefault(InvitationExceptionStatus.InvitationSent),
            byStatus.GetValueOrDefault(InvitationExceptionStatus.Applied),
            byStatus.GetValueOrDefault(InvitationExceptionStatus.Expired),
            byStatus.GetValueOrDefault(InvitationExceptionStatus.Cancelled)));
    }
}
