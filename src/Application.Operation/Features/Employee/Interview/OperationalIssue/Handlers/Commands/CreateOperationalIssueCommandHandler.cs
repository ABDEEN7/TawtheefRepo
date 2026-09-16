using System.Text.Json;
using Application.Operation.Features.Employee.Interview.OperationalIssue.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.OperationalIssue.Handlers.Commands;

public sealed class CreateOperationalIssueCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOperationalIssueCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateOperationalIssueCommand request, CancellationToken cancellationToken)
    {
        var appointmentExists = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .AnyAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (!appointmentExists)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        var issue = InterviewOperationalIssue.Create(
            request.AppointmentId, request.IssueType, request.Description, request.IsBlocking);
        await unitOfWork.GetEntityRepository<InterviewOperationalIssue>().AddAsync(issue, cancellationToken);

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewOperationalIssue),
            EntityId = issue.Id,
            Action = InterviewOperationalIssueAuditActions.Created,
            NewValues = JsonSerializer.Serialize(new { request.AppointmentId, request.IssueType, request.IsBlocking })
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(issue.Id);
    }
}
