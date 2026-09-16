using Application.Operation.Features.Employee.QuestionBankRequests.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Commands;

public sealed class CreateQuestionBankRequestCommandHandler(
    IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
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
            return Result.Fail<Guid>(ErrorsCodes.InvalidUserIdentifier);

        return await unitOfWork.ExecuteInTransactionAsync<IResult<Guid>>(async ct =>
        {
            var requests = unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet;
            var openConflict = await requests.AsNoTracking().AnyAsync(x =>
                x.RequestTypeId == QuestionBankRequestTypeIds.CREATE && OpenStatuses.Contains(x.StatusId) &&
                x.QuestionBank.QuestionBankTypeId == request.QuestionBankTypeId &&
                (request.QuestionBankTypeId != QuestionBankTypeIds.Specialized ||
                 (x.QuestionBank.ManagementId == request.ManagementId && x.QuestionBank.JobTitleId == request.JobTitleId)), ct);
            if (openConflict)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankCreationRequestAlreadyInProgress);

            var banks = unitOfWork.GetEntityRepository<QuestionBank>().DbSet;
            var bankConflict = await banks.AsNoTracking().AnyAsync(x =>
                x.QuestionBankTypeId == request.QuestionBankTypeId &&
                (request.QuestionBankTypeId != QuestionBankTypeIds.Specialized ||
                 (x.ManagementId == request.ManagementId && x.JobTitleId == request.JobTitleId)), ct);
            if (bankConflict)
                return Result.Fail<Guid>(ErrorsCodes.QuestionBankAlreadyExists);

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
                SubmittedById = userId,
                SubmittedAt = DateTime.UtcNow
            };
            await unitOfWork.GetEntityRepository<QuestionBankRequest>().AddAsync(questionBankRequest, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Result.Ok(questionBankRequest.Id);
        }, cancellationToken);
    }

    private static string? ValidateTarget(CreateQuestionBankRequestCommand request)
    {
        if (request.StageId.HasValue)
            return ErrorsCodes.QuestionBankCreationRequestStageNotSupported;

        if (request.QuestionBankTypeId == QuestionBankTypeIds.Specialized)
            return request.ManagementId.HasValue && request.JobTitleId.HasValue
                ? null
                : ErrorsCodes.SpecializedQuestionBankTargetRequired;

        if (request.QuestionBankTypeId is var type &&
            (type == QuestionBankTypeIds.Skills || type == QuestionBankTypeIds.Educational))
            return request.ManagementId.HasValue || request.JobTitleId.HasValue
                ? ErrorsCodes.QuestionBankTargetNotAllowed
                : null;

        return ErrorsCodes.InvalidQuestionBankType;
    }
}
