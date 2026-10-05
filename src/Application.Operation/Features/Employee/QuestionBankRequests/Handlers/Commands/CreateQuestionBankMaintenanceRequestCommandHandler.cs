using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Commands;

public sealed class CreateQuestionBankMaintenanceRequestCommandHandler(
    IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    : IRequestHandler<CreateQuestionBankMaintenanceRequestCommand, IResult<Guid>>
{
    private static readonly Guid[] OpenStatuses =
    [
        QuestionBankRequestStatusIds.PendingAssignment,
        QuestionBankRequestStatusIds.QuestionEntryInProgress,
        QuestionBankRequestStatusIds.PendingReview,
        QuestionBankRequestStatusIds.ModificationInProgress
    ];

    public async Task<IResult<Guid>> Handle(
        CreateQuestionBankMaintenanceRequestCommand command, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(currentUserService.UserId, out var userId))
            return Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<Guid>>(async ct =>
        {
            var bank = await unitOfWork.GetEntityRepository<QuestionBank>().DbSet
                .AsNoTracking()
                .Include(x => x.CurrentApprovedVersion)
                .SingleOrDefaultAsync(x => x.Id == command.QuestionBankId && !x.IsDeleted, ct);

            if (bank is null) return Result.Fail<Guid>(ErrorsCodes.QuestionBankNotFound);

            var version = bank.CurrentApprovedVersion;
            if (!bank.IsActive || bank.CurrentApprovedVersionId is null || version is null ||
                version.IsDeleted || version.QuestionBankId != bank.Id || version.ApprovedAt is null)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankNotMaintainable);

            var requests = unitOfWork.GetEntityRepository<QuestionBankRequest>();
            if (await requests.DbSet.AsNoTracking().AnyAsync(x =>
                    !x.IsDeleted && x.QuestionBankId == bank.Id && OpenStatuses.Contains(x.StatusId), ct))
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
