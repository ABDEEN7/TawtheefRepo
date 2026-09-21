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

public sealed class GetMemberEvaluationFormQueryHandler(IUnitOfWork unitOfWork, EvaluationAccessResolver accessResolver)
    : IRequestHandler<GetMemberEvaluationFormQuery, IResult<MemberEvaluationFormDto>>
{
    public async Task<IResult<MemberEvaluationFormDto>> Handle(GetMemberEvaluationFormQuery request, CancellationToken cancellationToken)
    {
        var appointment = await unitOfWork.GetEntityRepository<InterviewAppointment>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result.Fail<MemberEvaluationFormDto>(new Error(ErrorsCodes.InterviewAppointmentNotFound));

        var memberResult = await accessResolver.GetAssignedMemberAsync(appointment.InterviewCommitteeId, cancellationToken);
        if (memberResult.IsFailed)
            return Result.Fail<MemberEvaluationFormDto>(memberResult.Errors);
        var member = memberResult.Value;

        var committee = await unitOfWork.GetEntityRepository<InterviewCommittee>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == appointment.InterviewCommitteeId, cancellationToken);
        if (committee is null)
            return Result.Fail<MemberEvaluationFormDto>(new Error(ErrorsCodes.InterviewScheduleCommitteeNotFound));

        var versionResult = await EvaluationTemplateResolver.GetApprovedVersionAsync(unitOfWork, committee.InterviewTemplateId, cancellationToken);
        if (versionResult.IsFailed)
            return Result.Fail<MemberEvaluationFormDto>(versionResult.Errors);
        var version = versionResult.Value;

        var evaluation = await unitOfWork.GetEntityRepository<InterviewMemberEvaluation>().DbSet
            .AsNoTracking()
            .Include(e => e.CriterionScores)
            .FirstOrDefaultAsync(e => e.InterviewAppointmentId == appointment.Id && e.InterviewCommitteeMemberId == member.Id, cancellationToken);

        var allowedAxisIds = member.EvaluationScope == EvaluationScope.SelectedAxes
            ? member.EvaluationAxes.Select(a => a.InterviewTemplateEvaluationAxisId).ToHashSet()
            : version.Axes.Select(a => a.Id).ToHashSet();

        var axes = version.Axes
            .Where(a => allowedAxisIds.Contains(a.Id))
            .OrderBy(a => a.OrderNo)
            .Select(a => new MemberEvaluationFormAxisDto(
                a.Id,
                a.InterviewEvaluationAxis?.NameAr,
                a.InterviewEvaluationAxis?.NameEn,
                a.MaxScore,
                a.OrderNo,
                a.Criteria
                    .OrderBy(c => c.OrderNo)
                    .Select(c =>
                    {
                        var existing = evaluation?.CriterionScores.FirstOrDefault(s => s.InterviewTemplateEvaluationCriterionId == c.Id);
                        return new MemberEvaluationFormCriterionDto(
                            c.Id,
                            c.NameAr ?? c.InterviewEvaluationCriterion?.NameAr,
                            c.NameEn ?? c.InterviewEvaluationCriterion?.NameEn,
                            c.DescriptionAr ?? c.InterviewEvaluationCriterion?.DescriptionAr,
                            c.DescriptionEn ?? c.InterviewEvaluationCriterion?.DescriptionEn,
                            c.MaxScore,
                            c.IsRequired,
                            c.OrderNo,
                            existing?.Score,
                            existing?.Notes);
                    })
                    .ToList()))
            .ToList();

        var canEdit = appointment.Status is AppointmentStatus.InInterview or AppointmentStatus.UnderEvaluation
            && (evaluation is null || evaluation.Status == MemberEvaluationStatus.Draft);

        var dto = new MemberEvaluationFormDto(
            evaluation?.Id,
            appointment.Id,
            member.Id,
            evaluation?.Status,
            evaluation?.TotalScore,
            evaluation?.GeneralNotes,
            canEdit,
            axes);

        return Result.Ok(dto);
    }
}
