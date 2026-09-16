using System.Text.Json;
using Application.Operation.Features.Employee.Interview.Evaluation.Commands;
using Application.Operation.Features.Employee.Interview.Evaluation.Services;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Evaluation.Handlers.Commands;

public sealed class SaveMemberEvaluationDraftCommandHandler(IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver)
    : IRequestHandler<SaveMemberEvaluationDraftCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(SaveMemberEvaluationDraftCommand request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        if (appointment.Status is not (AppointmentStatus.InInterview or AppointmentStatus.UnderEvaluation))
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewAppointmentNotEvaluable));

        var memberResult = await accessResolver.GetAssignedMemberAsync(appointment.InterviewCommitteeId, cancellationToken);
        if (memberResult.IsFailed)
            return Result.Fail<Guid>(memberResult.Errors);
        var member = memberResult.Value;

        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == appointment.InterviewCommitteeId, cancellationToken);
        if (committee is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewScheduleCommitteeNotFound));

        var versionResult = await EvaluationTemplateResolver.GetApprovedVersionAsync(unitOfWork, committee.InterviewTemplateId, cancellationToken);
        if (versionResult.IsFailed)
            return Result.Fail<Guid>(versionResult.Errors);
        var version = versionResult.Value;

        // AllAxes members may score any axis in the version; SelectedAxes members are restricted to
        // the axes explicitly assigned to them on the committee.
        var allowedAxisIds = member.EvaluationScope == EvaluationScope.SelectedAxes
            ? member.EvaluationAxes.Select(a => a.InterviewTemplateEvaluationAxisId).ToHashSet()
            : version.Axes.Select(a => a.Id).ToHashSet();

        var allowedCriteria = version.Axes
            .Where(a => allowedAxisIds.Contains(a.Id))
            .SelectMany(a => a.Criteria)
            .ToDictionary(c => c.Id);

        var inputs = new List<MemberEvaluationCriterionScoreInput>(request.Scores.Count);
        foreach (var score in request.Scores)
        {
            if (!allowedCriteria.TryGetValue(score.CriterionId, out var criterion))
                return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewMemberEvaluationCriterionOutOfScope));

            inputs.Add(new MemberEvaluationCriterionScoreInput(score.CriterionId, score.Score, criterion.MaxScore, score.Notes));
        }

        var evaluation = await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().DbSet
            .Include(e => e.CriterionScores)
            .FirstOrDefaultAsync(e => e.InterviewAppointmentId == appointment.Id && e.InterviewCommitteeMemberId == member.Id, cancellationToken);

        var isNewEvaluation = evaluation is null;
        if (evaluation is null)
        {
            evaluation = InterviewMemberEvaluation.Create(appointment.Id, member.Id);
            await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().AddAsync(evaluation, cancellationToken);
        }

        var saveResult = evaluation.SaveDraft(inputs, request.GeneralNotes);
        if (saveResult.IsFailed)
            return Result.Fail<Guid>(saveResult.Errors);

        // The very first draft save by any member is what moves the appointment out of InInterview -
        // matches the entity's own comment ("MarkUnderEvaluation - Triggered by the first member draft save").
        if (isNewEvaluation && appointment.Status == AppointmentStatus.InInterview)
        {
            var markResult = appointment.MarkUnderEvaluation();
            if (markResult.IsFailed)
                return Result.Fail<Guid>(markResult.Errors);
        }

        await unitOfWork.GetEntityRepository<InterviewAuditLog>().AddAsync(new InterviewAuditLog
        {
            EntityType = nameof(InterviewMemberEvaluation),
            EntityId = evaluation.Id,
            Action = InterviewMemberEvaluationAuditActions.DraftSaved,
            NewValues = JsonSerializer.Serialize(new { request.AppointmentId, InterviewCommitteeMemberId = member.Id, ScoresCount = inputs.Count })
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(evaluation.Id);
    }
}
