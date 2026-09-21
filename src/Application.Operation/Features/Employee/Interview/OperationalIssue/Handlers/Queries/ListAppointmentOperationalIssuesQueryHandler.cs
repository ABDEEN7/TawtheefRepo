using Application.Operation.Features.Employee.Interview.OperationalIssue.DTOs;
using Application.Operation.Features.Employee.Interview.OperationalIssue.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Handlers.Queries;

public sealed class ListAppointmentOperationalIssuesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListAppointmentOperationalIssuesQuery, IResult<List<OperationalIssueDto>>>
{
    public async Task<IResult<List<OperationalIssueDto>>> Handle(ListAppointmentOperationalIssuesQuery request, CancellationToken cancellationToken)
    {
        var issues = await unitOfWork.GetEntityRepository<InterviewOperationalIssue>().DbSet
            .AsNoTracking()
            .Where(i => i.InterviewAppointmentId == request.AppointmentId)
            .OrderByDescending(i => i.CreatedDate)
            .Select(i => new OperationalIssueDto(
                i.Id, i.InterviewAppointmentId, i.IssueType, i.Description, i.IsBlocking, i.Status,
                i.ResolvedById, i.ResolvedAt, i.ResolutionNotes, i.CreatedDate))
            .ToListAsync(cancellationToken);

        return Result.Ok(issues);
    }
}
