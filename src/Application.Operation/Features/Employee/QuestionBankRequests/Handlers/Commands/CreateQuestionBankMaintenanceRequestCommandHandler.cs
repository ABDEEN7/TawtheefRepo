using Application.Operation.Features.Employee.QuestionBankRequests;
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

public sealed class CreateQuestionBankMaintenanceRequestCommandHandler(
    IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IRequestHandler<CreateQuestionBankMaintenanceRequestCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(
        CreateQuestionBankMaintenanceRequestCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            return Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier);
        var validEmployee = await unitOfWork.Context.Set<EmployeeUser>().AsNoTracking().AnyAsync(x =>
            x.Id == userId && !x.IsDeleted && !x.IsBlocked && x.EmployeeProfile != null &&
            !x.EmployeeProfile.IsDeleted, cancellationToken);
        if (!validEmployee) return Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<Guid>>(async ct =>
        {
            var bank = await unitOfWork.GetEntityRepository<QuestionBank>().DbSet
                .AsNoTracking()
                .Include(x => x.CurrentApprovedVersion)
                .SingleOrDefaultAsync(x => x.Id == command.QuestionBankId && !x.IsDeleted, ct);

            if (bank is null) return Result.Fail<Guid>(ErrorsCodes.QuestionBankNotFound);

            var maintainabilityError = QuestionBankMaintenanceRules.GetMaintainabilityError(bank);
            if (maintainabilityError is not null) return Result.Fail<Guid>(maintainabilityError);

            var requests = unitOfWork.GetEntityRepository<QuestionBankRequest>();
            if (await requests.DbSet.AsNoTracking().AnyAsync(x =>
                    !x.IsDeleted && x.QuestionBankId == bank.Id &&
                    QuestionBankMaintenanceRules.OpenStatuses.Contains(x.StatusId), ct))
            {
                return Result.Fail<Guid>(new Error(ErrorsCodes.QuestionBankActiveRequestAlreadyExists)
                    .WithMetadata("Code", "Conflict")
                    .WithMetadata("UserMessage", "There is already an active question bank request for this bank."));
            }

            var entity = new QuestionBankRequest
            {
                QuestionBankId = bank.Id,
                BaseVersionId = bank.CurrentApprovedVersionId,
                RequestTypeId = QuestionBankRequestTypeIds.MAINTENANCE,
                StatusId = QuestionBankRequestStatusIds.PendingAssignment,
                CurrentReviewRound = 0,
                Reason = command.Reason?.Trim(),
                SubmittedById = userId,
                SubmittedAt = DateTime.UtcNow,
                FinalDecisionById = null,
                FinalDecisionAt = null,
                FinalDecisionNote = null
            };
            await requests.AddAsync(entity, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Ok(entity.Id);
        }, cancellationToken);
    }
}
