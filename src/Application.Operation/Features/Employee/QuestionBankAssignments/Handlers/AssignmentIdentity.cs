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

internal static class AssignmentIdentity
{
    public static async Task<Guid?> CurrentEmployeeId(IUnitOfWork uow, ICurrentUserService currentUser, CancellationToken ct)
    {
        if (!Guid.TryParse(currentUser.UserId, out var userId)) return null;
        // QuestionBankAssignment's configured FK targets EmployeeUser.Id; resolve through the authenticated employee user.
        return await uow.Context.Set<EmployeeUser>().AsNoTracking()
            .Where(x => x.Id == userId && !x.IsDeleted && !x.IsBlocked && x.EmployeeProfile != null && !x.EmployeeProfile.IsDeleted)
            .Select(x => (Guid?)x.Id).SingleOrDefaultAsync(ct);
    }
}
