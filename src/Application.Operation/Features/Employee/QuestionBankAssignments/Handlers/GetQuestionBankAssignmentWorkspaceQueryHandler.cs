using Application.Operation.Features.Employee.QuestionBankAssignments.Commands;
using Application.Operation.Features.Employee.QuestionBankAssignments.DTOs;
using Application.Operation.Features.Employee.QuestionBankAssignments.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

public sealed class GetQuestionBankAssignmentWorkspaceQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<GetQuestionBankAssignmentWorkspaceQuery, IResult<QuestionBankAssignmentWorkspaceDto>>
{
    public async Task<IResult<QuestionBankAssignmentWorkspaceDto>> Handle(GetQuestionBankAssignmentWorkspaceQuery r, CancellationToken ct)
    {
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, ct);
        if (employeeId is null) return Result.Fail<QuestionBankAssignmentWorkspaceDto>(ErrorsCodes.InvalidUserIdentifier);
        var a = await uow.GetEntityRepository<QuestionBankAssignment>().DbSet.AsNoTracking()
            .Include(x => x.Status).Include(x => x.QuestionBankRequest).ThenInclude(x => x.QuestionBank).ThenInclude(x => x.QuestionBankType)
            .Include(x => x.QuestionBankRequest.QuestionBank.Management).Include(x => x.QuestionBankRequest.QuestionBank.JobTitle)
            .SingleOrDefaultAsync(x => x.Id == r.AssignmentId && x.EmployeeId == employeeId && !x.IsDeleted, ct);
        if (a is null) return Result.Fail<QuestionBankAssignmentWorkspaceDto>(ErrorsCodes.QuestionBankAssignmentNotFound);
        var questions = await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet.AsNoTracking()
            .Where(x => x.QuestionBankAssignmentId == a.Id && x.RequestId == a.QuestionBankRequestId && !x.IsDeleted &&
                        x.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST)
            .OrderBy(x => x.CreatedDate).Select(x => new AssignmentQuestionDto(x.Id, x.QuestionId,
                x.CurrentProposedRevision!.Id, x.CurrentProposedRevision.RevisionNo, x.CurrentProposedRevision.QuestionTypeId,
                x.CurrentProposedRevision.QuestionType.NameAr, x.CurrentProposedRevision.QuestionType.NameEn,
                x.CurrentProposedRevision.DifficultyLevelId, x.CurrentProposedRevision.DifficultyLevel.NameAr,
                x.CurrentProposedRevision.DifficultyLevel.NameEn, x.CurrentProposedRevision.QuestionTextAr,
                x.CurrentProposedRevision.QuestionTextEn, x.CurrentProposedRevision.ExplanationAr,
                x.CurrentProposedRevision.ExplanationEn, x.StatusId, x.Status.NameAr, x.Status.NameEn,
                x.CurrentProposedRevision.Options.OrderBy(o => o.DisplayOrder)
                    .Select(o => new QuestionOptionDto(o.Id, o.OptionTextAr, o.OptionTextEn, o.IsCorrect, o.DisplayOrder)).ToList())).ToListAsync(ct);
        var current = questions.Count; var bank = a.QuestionBankRequest.QuestionBank;
        return Result.Ok(new QuestionBankAssignmentWorkspaceDto(
            new(a.Id, a.StatusId, a.Status.NameAr, a.Status.NameEn, a.MinimumQuestionCount, a.Notes, a.AssignedAt, a.QuestionEntryStartedAt, a.QuestionEntryCompletedAt),
            new(a.QuestionBankRequestId, a.QuestionBankRequest.StatusId),
            new(bank.QuestionBankTypeId, bank.QuestionBankType.NameAr, bank.QuestionBankType.NameEn,
                bank.Management?.NameAr, bank.Management?.NameEn, bank.JobTitle?.JobNameAr, bank.JobTitle?.JobNameEn),
            new(a.MinimumQuestionCount, current, Math.Max(0, a.MinimumQuestionCount-current), current >= a.MinimumQuestionCount && current > 0), questions));
    }
}
