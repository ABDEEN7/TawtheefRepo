using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using Application.Operation.Features.Employee.Interview.Schedule.DTOs;
using Application.Operation.Features.Employee.Interview.Schedule.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Handlers.Queries;

public sealed class ListMySessionAppointmentsQueryHandler(
    IUnitOfWork unitOfWork, IMediator mediator, EvaluationSessionAccessResolver sessionAccess)
    : IRequestHandler<ListMySessionAppointmentsQuery, IResult<List<AppointmentDto>>>
{
    public async Task<IResult<List<AppointmentDto>>> Handle(ListMySessionAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var jobId = await unitOfWork.GetEntityRepository<InterviewSchedule>().DbSet
            .AsNoTracking()
            .Where(s => s.Id == request.ScheduleId)
            .Select(s => (Guid?)s.JobId)
            .FirstOrDefaultAsync(cancellationToken);
        if (jobId is null)
            return Result.Fail<List<AppointmentDto>>(new Error(ErrorsCodes.InterviewScheduleNotFound));

        var canAccess = await sessionAccess.CanAccessJobAsync(jobId.Value, cancellationToken);
        if (!canAccess)
            return Result.Fail<List<AppointmentDto>>(new Error(ErrorsCodes.InterviewEvaluationSessionAccessDenied)
                .WithMetadata("StatusCode", StatusCodes.Status403Forbidden));

        return await mediator.Send(new ListScheduleAppointmentsQuery(request.ScheduleId), cancellationToken);
    }
}
