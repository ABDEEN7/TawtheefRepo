using System.Linq.Expressions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

internal static class AssignmentAccess
{
    // Entry completion is a submission for review. Only final workflow completion
    // or issuance closes employee access; returned assignments remain accessible.
    public static readonly Expression<Func<QuestionBankAssignment, bool>> Accessible = a =>
        a.StatusId != QuestionBankAssignmentStatusIds.Completed &&
        a.QuestionBankRequest.StatusId != QuestionBankRequestStatusIds.Issued;

    private static readonly Func<QuestionBankAssignment, bool> IsAccessible = Accessible.Compile();

    public static bool CanAccess(QuestionBankAssignment assignment) => IsAccessible(assignment);

    public static Error Forbidden() => new Error(ErrorsCodes.QuestionBankAssignmentNotEditable)
        .WithMetadata("StatusCode", StatusCodes.Status403Forbidden);

    public static async Task<IResult<Unit>> ValidateAsync(
        IUnitOfWork uow, Guid assignmentId, Guid employeeId, CancellationToken ct)
    {
        var assignments = uow.GetEntityRepository<QuestionBankAssignment>().DbSet.AsNoTracking()
            .Where(a => a.Id == assignmentId && a.EmployeeId == employeeId && !a.IsDeleted);
        if (!await assignments.AnyAsync(ct))
            return Result.Fail<Unit>(new Error(ErrorsCodes.QuestionBankAssignmentNotFound)
                .WithMetadata("StatusCode", StatusCodes.Status404NotFound));
        return await assignments.Where(Accessible).AnyAsync(ct)
            ? Result.Ok(Unit.Value)
            : Result.Fail<Unit>(Forbidden());
    }
}
