using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Evaluation.Commands;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using Application.Operation.Features.Employee.Interview.ResultReport.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Handlers.Commands;

public sealed class SubmitMemberEvaluationCommandHandler(IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver)
    : IRequestHandler<SubmitMemberEvaluationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SubmitMemberEvaluationCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        if (appointment.Status is not (AppointmentStatus.InInterview or AppointmentStatus.UnderEvaluation))
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewAppointmentNotEvaluable));

        var memberResult = await accessResolver.GetAssignedMemberAsync(appointment.InterviewCommitteeId, cancellationToken);
        if (memberResult.IsFailed)
            return Result.Fail<Unit>(memberResult.Errors);
        var member = memberResult.Value;

        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == appointment.InterviewCommitteeId, cancellationToken);
        if (committee is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewScheduleCommitteeNotFound));

        var versionResult = await EvaluationTemplateResolver.GetApprovedVersionAsync(unitOfWork, committee.InterviewTemplateId, cancellationToken);
        if (versionResult.IsFailed)
            return Result.Fail<Unit>(versionResult.Errors);
        var version = versionResult.Value;

        var allowedAxisIds = member.EvaluationScope == EvaluationScope.SelectedAxes
            ? member.EvaluationAxes.Select(a => a.InterviewTemplateEvaluationAxisId).ToHashSet()
            : version.Axes.Select(a => a.Id).ToHashSet();

        var requiredCriterionIds = version.Axes
            .Where(a => allowedAxisIds.Contains(a.Id))
            .SelectMany(a => a.Criteria)
            .Where(c => c.IsRequired)
            .Select(c => c.Id)
            .ToHashSet();

        var evaluation = await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().DbSet
            .Include(e => e.CriterionScores)
            .FirstOrDefaultAsync(e => e.InterviewAppointmentId == appointment.Id && e.InterviewCommitteeMemberId == member.Id, cancellationToken);
        if (evaluation is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewMemberEvaluationNotFound));

        var submitResult = evaluation.Submit(requiredCriterionIds);
        if (submitResult.IsFailed)
            return Result.Fail<Unit>(submitResult.Errors);

        // Quorum check: all active, ParticipatesInEvaluation members must be Submitted for this
        // appointment before it auto-completes. The DB hasn't seen this member's own Submit yet (no
        // SaveChanges so far), so their contribution is counted in memory rather than re-queried.
        var requiredMemberIds = await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
            .Where(m => m.InterviewCommitteeId == appointment.InterviewCommitteeId && m.IsActive && m.ParticipatesInEvaluation)
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);

        var otherSubmittedCount = await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().DbSet
            .CountAsync(e => e.InterviewAppointmentId == appointment.Id
                && e.InterviewCommitteeMemberId != member.Id
                && requiredMemberIds.Contains(e.InterviewCommitteeMemberId)
                && e.Status == MemberEvaluationStatus.Submitted, cancellationToken);

        var currentMemberSubmittedCount = requiredMemberIds.Contains(member.Id) ? 1 : 0;
        var allSubmitted = requiredMemberIds.Count > 0
            && otherSubmittedCount + currentMemberSubmittedCount >= requiredMemberIds.Count;

        if (allSubmitted)
        {
            var completeResult = appointment.CompleteEvaluation();
            if (completeResult.IsFailed)
                return Result.Fail<Unit>(completeResult.Errors);

            // Was this the last live appointment in the schedule to finish evaluation? If so, generate
            // the whole schedule's InterviewResultReport in the same transaction. Always Ok() on early
            // exits (not ready yet / already generated / unsupported calculation method) - a legitimate
            // member Submit must never fail because of schedule-wide state it doesn't control.
            var generateResult = await InterviewResultCalculationService.TryGenerateReportIfScheduleDoneAsync(
                unitOfWork, appointment.InterviewScheduleId, cancellationToken);
            if (generateResult.IsFailed)
                return Result.Fail<Unit>(generateResult.Errors);
        }

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewMemberEvaluation),
            EntityId = evaluation.Id,
            Action = InterviewMemberEvaluationAuditActions.Submitted,
            NewValues = JsonSerializer.Serialize(new { request.AppointmentId, InterviewCommitteeMemberId = member.Id, evaluation.TotalScore })
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
