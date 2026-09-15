using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Commands;

public sealed class CreateQuestionBankRequestCommandHandler(
    IUnitOfWork unitOfWork, ICurrentUserService currentUserService, UserManager<User> userManager)
    : IRequestHandler<CreateQuestionBankRequestCommand, IResult<Guid>>
{
    private static readonly Guid[] OpenStatuses =
    [
        QuestionBankRequestStatusIds.PendingAssignment,
        QuestionBankRequestStatusIds.QuestionEntryInProgress,
        QuestionBankRequestStatusIds.PendingReview,
        QuestionBankRequestStatusIds.ModificationInProgress
    ];

    public async Task<IResult<Guid>> Handle(CreateQuestionBankRequestCommand request, CancellationToken cancellationToken)
    {
        var validation = ValidateTarget(request);
        if (validation is not null) return Result.Fail<Guid>(validation);

        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            return Result.Fail<Guid>(new Error("Unauthorized").WithMetadata("Code", "Unauthorized"));

        var employeeProfileId = await userManager.Users.AsNoTracking()
            .OfType<EmployeeUser>()
            .Where(x => x.Id == userId)
            .Select(x => (Guid?)x.EmployeeProfileId)
            .SingleOrDefaultAsync(cancellationToken);
        if (!employeeProfileId.HasValue)
            return Result.Fail<Guid>(new Error("Unauthorized").WithMetadata("Code", "Unauthorized"));

        return await unitOfWork.ExecuteInTransactionAsync<IResult<Guid>>(async ct =>
        {
            var requests = unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet;
            var openConflict = await requests.AsNoTracking().AnyAsync(x =>
                x.RequestTypeId == QuestionBankRequestTypeIds.CREATE && OpenStatuses.Contains(x.StatusId) &&
                x.QuestionBank.QuestionBankTypeId == request.QuestionBankTypeId &&
                (request.QuestionBankTypeId != QuestionBankTypeIds.SPECIALIZED ||
                 (x.QuestionBank.ManagementId == request.ManagementId && x.QuestionBank.JobTitleId == request.JobTitleId)), ct);
            if (openConflict)
                return Conflict("A question bank creation request is already in progress for the selected data.");

            var banks = unitOfWork.GetEntityRepository<QuestionBank>().DbSet;
            var bankConflict = await banks.AsNoTracking().AnyAsync(x =>
                x.QuestionBankTypeId == request.QuestionBankTypeId &&
                (request.QuestionBankTypeId != QuestionBankTypeIds.SPECIALIZED ||
                 (x.ManagementId == request.ManagementId && x.JobTitleId == request.JobTitleId)), ct);
            if (bankConflict)
                return Conflict("A question bank already exists for the selected data. Please submit a maintenance request for the existing bank.");

            var bank = new QuestionBank
            {
                QuestionBankTypeId = request.QuestionBankTypeId,
                ManagementId = request.ManagementId,
                JobTitleId = request.JobTitleId,
                StageId = null,
                IsActive = false,
                CurrentApprovedVersionId = null
            };
            await unitOfWork.GetEntityRepository<QuestionBank>().AddAsync(bank, ct);

            var questionBankRequest = new QuestionBankRequest
            {
                QuestionBankId = bank.Id,
                BaseVersionId = null,
                RequestTypeId = QuestionBankRequestTypeIds.CREATE,
                StatusId = QuestionBankRequestStatusIds.PendingAssignment,
                CurrentReviewRound = 0,
                Reason = request.Reason?.Trim(),
                SubmittedById = employeeProfileId.Value,
                SubmittedAt = DateTime.UtcNow
            };
            await unitOfWork.GetEntityRepository<QuestionBankRequest>().AddAsync(questionBankRequest, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Ok(questionBankRequest.Id);
        }, cancellationToken);
    }

    private static Error? ValidateTarget(CreateQuestionBankRequestCommand request)
    {
        if (request.StageId.HasValue)
            return Validation("Stage is not supported for question bank creation requests.");

        if (request.QuestionBankTypeId == QuestionBankTypeIds.SPECIALIZED)
            return request.ManagementId.HasValue && request.JobTitleId.HasValue
                ? null
                : Validation("Management and job title are required for specialized question banks.");

        if (request.QuestionBankTypeId is var type &&
            (type == QuestionBankTypeIds.SKILLS || type == QuestionBankTypeIds.EDUCATIONAL))
            return request.ManagementId.HasValue || request.JobTitleId.HasValue
                ? Validation("Management and job title must be empty for this question bank type.")
                : null;

        return Validation("Invalid question bank type.");
    }

    private static IResult<Guid> Conflict(string message) =>
        Result.Fail<Guid>(new Error(message).WithMetadata("Code", "Conflict").WithMetadata("UserMessage", message));

    private static Error Validation(string message) =>
        new Error(message).WithMetadata("Code", "Validation").WithMetadata("UserMessage", message);
}
