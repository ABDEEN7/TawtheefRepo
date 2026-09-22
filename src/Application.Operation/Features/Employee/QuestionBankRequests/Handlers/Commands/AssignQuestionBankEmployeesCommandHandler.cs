using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Commands;

public sealed class AssignQuestionBankEmployeesCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IRequestHandler<AssignQuestionBankEmployeesCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(AssignQuestionBankEmployeesCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var currentUserId))
            return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);

        var currentEmployeeUserExists = await unitOfWork.Context.Set<EmployeeUser>().AsNoTracking()
            .AnyAsync(x => x.Id == currentUserId && !x.IsDeleted && !x.IsBlocked, cancellationToken);
        if (!currentEmployeeUserExists)
            return Result.Fail<Unit>(ErrorsCodes.InvalidUserIdentifier);
        if (command.Assignments.Count == 0) return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentsRequired);
        if (command.Assignments.Any(x => x.EmployeeId == Guid.Empty || x.MinimumQuestionCount <= 0 || x.Notes?.Length > 1000))
            return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentsRequired);
        var employeeIds = command.Assignments.Select(x => x.EmployeeId).ToArray();
        if (employeeIds.Distinct().Count() != employeeIds.Length) return Result.Fail<Unit>(ErrorsCodes.DuplicateQuestionBankAssignee);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async ct =>
        {
            var request = await unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet
                .Include(x => x.QuestionBank).SingleOrDefaultAsync(x => x.Id == command.RequestId, ct);
            if (request is null) return Result.Fail<Unit>(ErrorsCodes.QuestionBankRequestNotFound);
            if (request.StatusId != QuestionBankRequestStatusIds.PendingAssignment &&
                request.StatusId != QuestionBankRequestStatusIds.QuestionEntryInProgress)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankRequestNotPendingAssignment);

            if (await unitOfWork.GetEntityRepository<QuestionBankAssignment>().DbSet.AsNoTracking()
                    .AnyAsync(x => x.QuestionBankRequestId == request.Id && employeeIds.Contains(x.EmployeeId), ct))
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssignmentAlreadyExists);

            var eligibleEmployees = unitOfWork.Context.Set<EmployeeUser>().AsNoTracking()
                .Where(x => employeeIds.Contains(x.Id) && !x.IsDeleted && !x.IsBlocked &&
                            x.EmployeeProfile != null && !x.EmployeeProfile.IsDeleted);
            if (request.QuestionBank.QuestionBankTypeId == QuestionBankTypeIds.Specialized)
            {
                if (request.QuestionBank.ManagementId is null) return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssigneeNotEligible);
                var departmentNumber = await unitOfWork.Context.Set<Management>().AsNoTracking()
                    .Where(x => x.Id == request.QuestionBank.ManagementId)
                    .Select(x => x.DepartmentNumber)
                    .SingleOrDefaultAsync(ct);
                if (string.IsNullOrWhiteSpace(departmentNumber))
                    return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssigneeNotEligible);
                eligibleEmployees = eligibleEmployees.Where(x =>
                    x.EmployeeProfile!.DepartmentNumber == departmentNumber);
            }
            else
            {
                // No organization-wide scope rule exists for global banks yet; fail closed.
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankEmployeeScopeNotConfigured);
            }

            if (await eligibleEmployees.CountAsync(ct) != employeeIds.Length)
                return Result.Fail<Unit>(ErrorsCodes.QuestionBankAssigneeNotEligible);

            var now = DateTime.UtcNow;
            var assignmentRepo = unitOfWork.GetEntityRepository<QuestionBankAssignment>();
            foreach (var input in command.Assignments)
                await assignmentRepo.AddAsync(new QuestionBankAssignment
                {
                    QuestionBankRequestId = request.Id, EmployeeId = input.EmployeeId,
                    AssignedByUserId = currentUserId, StatusId = QuestionBankAssignmentStatusIds.Assigned,
                    MinimumQuestionCount = input.MinimumQuestionCount, Notes = input.Notes?.Trim(), AssignedAt = now
                }, ct);

            if (request.StatusId == QuestionBankRequestStatusIds.PendingAssignment)
            {
                var oldStatus = request.StatusId;
                request.StatusId = QuestionBankRequestStatusIds.QuestionEntryInProgress;
                await unitOfWork.GetEntityRepository<QuestionBankRequestHistory>().AddAsync(new QuestionBankRequestHistory
                {
                    RequestId = request.Id, FromStatusId = oldStatus, ToStatusId = request.StatusId,
                    Action = "EmployeesAssigned", PerformedById = currentUserId, PerformedAt = now
                }, ct);
            }
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Ok(Unit.Value);
        }, cancellationToken);
    }
}
