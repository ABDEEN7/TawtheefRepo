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

public sealed class ListMyQuestionBankAssignmentsQueryHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<ListMyQuestionBankAssignmentsQuery, IResult<PaginatedResult<MyQuestionBankAssignmentDto>>>
{
    public async Task<IResult<PaginatedResult<MyQuestionBankAssignmentDto>>> Handle(ListMyQuestionBankAssignmentsQuery r, CancellationToken ct)
    {
        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, ct);
        if (employeeId is null) return Result.Fail<PaginatedResult<MyQuestionBankAssignmentDto>>(ErrorsCodes.InvalidUserIdentifier);
        var query = uow.GetEntityRepository<QuestionBankAssignment>().DbSet.AsNoTracking()
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted).OrderByDescending(x => x.AssignedAt)
            .Select(x => new MyQuestionBankAssignmentDto(x.Id, x.QuestionBankRequestId,
                x.QuestionBankRequest.QuestionBank.QuestionBankTypeId, x.QuestionBankRequest.QuestionBank.QuestionBankType.NameAr,
                x.QuestionBankRequest.QuestionBank.QuestionBankType.NameEn, x.QuestionBankRequest.QuestionBank.Management == null ? null : x.QuestionBankRequest.QuestionBank.Management.NameAr,
                x.QuestionBankRequest.QuestionBank.Management == null ? null : x.QuestionBankRequest.QuestionBank.Management.NameEn,
                x.QuestionBankRequest.QuestionBank.JobTitle == null ? null : x.QuestionBankRequest.QuestionBank.JobTitle.JobNameAr,
                x.QuestionBankRequest.QuestionBank.JobTitle == null ? null : x.QuestionBankRequest.QuestionBank.JobTitle.JobNameEn,
                x.StatusId, x.Status.NameAr, x.Status.NameEn, x.MinimumQuestionCount,
                x.RequestItems.Count(i => !i.IsDeleted && i.RequestId == x.QuestionBankRequestId && i.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST),
                x.AssignedAt, x.QuestionEntryStartedAt, x.QuestionEntryCompletedAt));
        var count = await query.CountAsync(ct);
        var items = await query.Skip((r.PageNumber - 1) * r.PageSize).Take(r.PageSize).ToListAsync(ct);
        return Result.Ok(new PaginatedResult<MyQuestionBankAssignmentDto>(items, count, r.PageNumber, r.PageSize));
    }
}
