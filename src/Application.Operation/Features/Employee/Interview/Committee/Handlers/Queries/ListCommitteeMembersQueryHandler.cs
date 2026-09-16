using Application.Operation.Features.Employee.Interview.Committee.DTOs;
using Application.Operation.Features.Employee.Interview.Committee.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Employee.Interview.Committee.Handlers.Queries;

public sealed class ListCommitteeMembersQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ListCommitteeMembersQuery, IResult<List<CommitteeMemberDto>>>
{
    public async Task<IResult<List<CommitteeMemberDto>>> Handle(ListCommitteeMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await unitOfWork.GetEntityRepository<InterviewCommitteeMember>().DbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(m => m.MemberUser)
            .Include(m => m.EvaluationAxes).ThenInclude(a => a.InterviewTemplateEvaluationAxis!).ThenInclude(a => a.InterviewEvaluationAxis)
            .Where(m => m.InterviewCommitteeId == request.InterviewCommitteeId && m.IsActive)
            .ToListAsync(cancellationToken);

        var dtos = members
            .Select(m => new CommitteeMemberDto(
                m.Id,
                m.InterviewCommitteeId,
                m.MemberUserId,
                m.MemberUser!.FullNameAr,
                m.MemberUser.FullNameEn,
                m.Role,
                m.ParticipatesInEvaluation,
                m.EvaluationScope,
                m.CanViewCandidates,
                m.CanAddNotes,
                m.CanSubmitEvaluation,
                m.CanViewOtherEvaluations,
                m.CanViewCommitteeSummary,
                m.IsActive,
                m.RemovedAt,
                m.RemovalReason,
                m.EvaluationAxes
                    .Select(a => new CommitteeMemberEvaluationAxisDto(
                        a.Id,
                        a.InterviewTemplateEvaluationAxisId,
                        a.InterviewTemplateEvaluationAxis!.InterviewEvaluationAxis!.NameAr,
                        a.InterviewTemplateEvaluationAxis.InterviewEvaluationAxis.NameEn))
                    .ToList()))
            .ToList();

        return Result.Ok(dtos);
    }
}
