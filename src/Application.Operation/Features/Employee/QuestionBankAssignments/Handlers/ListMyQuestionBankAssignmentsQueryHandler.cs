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
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted);
        var countableItems = uow.GetEntityRepository<QuestionBankRequestItem>().DbSet.AsNoTracking()
            .Where(QuestionAssignmentProgress.Countable);
        var sorted = ApplySorting(query, countableItems, r.SortBy, r.SortDirection);
        var projected = sorted
            .Select(x => new MyQuestionBankAssignmentDto(x.Id, x.QuestionBankRequestId,
                x.QuestionBankRequest.QuestionBank.QuestionBankTypeId, x.QuestionBankRequest.QuestionBank.QuestionBankType.NameAr,
                x.QuestionBankRequest.QuestionBank.QuestionBankType.NameEn, x.QuestionBankRequest.QuestionBank.Management == null ? null : x.QuestionBankRequest.QuestionBank.Management.NameAr,
                x.QuestionBankRequest.QuestionBank.Management == null ? null : x.QuestionBankRequest.QuestionBank.Management.NameEn,
                x.QuestionBankRequest.QuestionBank.JobTitle == null ? null : x.QuestionBankRequest.QuestionBank.JobTitle.JobNameAr,
                x.QuestionBankRequest.QuestionBank.JobTitle == null ? null : x.QuestionBankRequest.QuestionBank.JobTitle.JobNameEn,
                x.StatusId, x.Status.NameAr, x.Status.NameEn, x.MinimumQuestionCount,
                countableItems.Count(i => i.QuestionBankAssignmentId == x.Id && i.RequestId == x.QuestionBankRequestId),
                x.AssignedAt, x.QuestionEntryStartedAt, x.QuestionEntryCompletedAt));
        var count = await projected.CountAsync(ct);
        var items = await projected.Skip((r.PageNumber - 1) * r.PageSize).Take(r.PageSize).ToListAsync(ct);
        return Result.Ok(new PaginatedResult<MyQuestionBankAssignmentDto>(items, count, r.PageNumber, r.PageSize));
    }

    private static IOrderedQueryable<QuestionBankAssignment> ApplySorting(
        IQueryable<QuestionBankAssignment> query,
        IQueryable<QuestionBankRequestItem> countableItems,
        string? sortBy,
        string? sortDirection)
    {
        var descending = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        return sortBy?.Trim().ToLowerInvariant() switch
        {
            "questionbanktype" => Order(query, x => x.QuestionBankRequest.QuestionBank.QuestionBankType.NameEn, descending),
            "management" => Order(query, x => x.QuestionBankRequest.QuestionBank.Management == null ? string.Empty : x.QuestionBankRequest.QuestionBank.Management.NameEn, descending),
            "jobtitle" => Order(query, x => x.QuestionBankRequest.QuestionBank.JobTitle == null ? string.Empty : x.QuestionBankRequest.QuestionBank.JobTitle.JobNameEn, descending),
            "minimumquestioncount" => Order(query, x => x.MinimumQuestionCount, descending),
            "currentquestioncount" => Order(query, x => countableItems.Count(i =>
                i.QuestionBankAssignmentId == x.Id && i.RequestId == x.QuestionBankRequestId), descending),
            "status" => Order(query, x => x.Status.DisplayOrder, descending),
            "assignedat" => Order(query, x => x.AssignedAt, descending),
            _ => query.OrderByDescending(x => x.AssignedAt).ThenBy(x => x.Id)
        };
    }

    private static IOrderedQueryable<QuestionBankAssignment> Order<TKey>(
        IQueryable<QuestionBankAssignment> query,
        System.Linq.Expressions.Expression<Func<QuestionBankAssignment, TKey>> key,
        bool descending) => descending
            ? query.OrderByDescending(key).ThenBy(x => x.Id)
            : query.OrderBy(key).ThenBy(x => x.Id);
}
