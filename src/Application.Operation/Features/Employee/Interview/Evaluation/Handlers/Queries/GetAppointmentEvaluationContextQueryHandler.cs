using Application.Operation.Features.Employee.Interview.Evaluation.DTOs;
using Application.Operation.Features.Employee.Interview.Evaluation.Queries;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Handlers.Queries;

public sealed class GetAppointmentEvaluationContextQueryHandler(IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver)
    : IRequestHandler<GetAppointmentEvaluationContextQuery, IResult<AppointmentEvaluationContextDto>>
{
    public async Task<IResult<AppointmentEvaluationContextDto>> Handle(GetAppointmentEvaluationContextQuery request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<AppointmentEvaluationContextDto>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        var bypass = accessResolver.HasSummaryRoleBypass();

        // requireCanSubmit: false - unlike the scoring form, viewer context must resolve for a Chair
        // who doesn't score, not just for members allowed to submit.
        var memberResult = await accessResolver.GetAssignedMemberAsync(
            appointment.InterviewCommitteeId, cancellationToken, requireCanSubmit: false);
        var member = memberResult.IsSuccess ? memberResult.Value : null;

        var isChair = member?.Role == CommitteeRole.Chair;

        var dto = new AppointmentEvaluationContextDto(
            member?.Id,
            member?.Role,
            member?.CanSubmitEvaluation ?? false,
            member?.CanViewCommitteeSummary ?? false,
            isChair || bypass);

        return Result.Ok(dto);
    }
}
