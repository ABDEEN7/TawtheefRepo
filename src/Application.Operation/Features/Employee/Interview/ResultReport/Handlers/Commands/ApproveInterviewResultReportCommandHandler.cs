using System.Text.Json;
using Application.Operation.Features.Employee.Interview.ResultReport.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Interview.ResultReport.Handlers.Commands;

public sealed class ApproveInterviewResultReportCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IRequestHandler<ApproveInterviewResultReportCommand, IResult<Unit>>
{
    private static readonly IReadOnlyDictionary<FinalDecision, Guid> DecisionToInvitationStatus = new Dictionary<FinalDecision, Guid>
    {
        [FinalDecision.CandidateForHiringProcess] = InvitationStatusIds.CandidateForHiringProcess,
        [FinalDecision.WaitingList] = InvitationStatusIds.WaitingList,
        [FinalDecision.Rejected] = InvitationStatusIds.Rejected
    };

    public async Task<IResult<Unit>> Handle(ApproveInterviewResultReportCommand request, CancellationToken cancellationToken)
    {
        var report = await unitOfWork.GetEntityRepository<InterviewResultReport>().DbSet
            .Include(r => r.Candidates)
            .FirstOrDefaultAsync(r => r.Id == request.ReportId, cancellationToken);
        if (report is null)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewResultReportNotFound));

        var appointmentIds = report.Candidates.Select(c => c.InterviewAppointmentId).ToList();

        // Cross-cutting check the entity cannot perform itself - the operational-issue block.
        var hasBlockingIssue = await unitOfWork.GetEntityRepository<InterviewOperationalIssue>().DbSet
            .AnyAsync(i => appointmentIds.Contains(i.InterviewAppointmentId)
                && i.IsBlocking && i.Status == OperationalIssueStatus.Open, cancellationToken);
        if (hasBlockingIssue)
            return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewResultReportBlockedByOperationalIssue));

        var appointments = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .Include(a => a.Invitation)
            .Where(a => appointmentIds.Contains(a.Id))
            .ToListAsync(cancellationToken);

        _ = Guid.TryParse(currentUserService.UserId, out var approverId);

        var decisions = request.Decisions.ToDictionary(d => d.CandidateId, d => (d.Decision, d.Reason));

        var approveResult = report.Approve(approverId, decisions);
        if (approveResult.IsFailed)
            return Result.Fail<Unit>(approveResult.Errors);

        // Propagates the decision into the recruitment pipeline (scheduleResult.md: "sync inv.status = c.finalDecision").
        foreach (var candidate in report.Candidates)
        {
            if (candidate.FinalDecision is null)
                continue;

            if (!DecisionToInvitationStatus.TryGetValue(candidate.FinalDecision.Value, out var invitationStatusId))
                return Result.Fail<Unit>(new Error(ErrorsCodes.InterviewResultReportMissingInvitationStatusMapping));

            var appointment = appointments.FirstOrDefault(a => a.Id == candidate.InterviewAppointmentId);
            appointment?.Invitation?.ChangeInvitationStatus(invitationStatusId);
        }

        // Per-candidate decision detail (not just a count) - lets anyone reviewing the audit trail see
        // exactly which decision was made for which candidate, by whom and when (user/timestamp come
        // from InterviewAuditLog's own CreatedById/CreatedDate, populated the same way as every other
        // audit row in this codebase). No separate "suggestion vs final decision" model is persisted -
        // the suggestion itself is never stored, only ever computed live at review time.
        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewResultReport),
            EntityId = report.Id,
            Action = InterviewResultReportAuditActions.Approved,
            NewValues = JsonSerializer.Serialize(new
            {
                request.ReportId,
                Decisions = report.Candidates.Select(c => new { CandidateId = c.Id, c.InterviewAppointmentId, c.FinalDecision, c.DecisionReason })
            })
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
