using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using Application.Operation.Features.Employee.QuestionBankRequests.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Queries;

public sealed class GetQuestionBankRequestDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetQuestionBankRequestDetailsQuery, IResult<QuestionBankRequestDetailsDto>>
{
    public async Task<IResult<QuestionBankRequestDetailsDto>> Handle(GetQuestionBankRequestDetailsQuery request, CancellationToken cancellationToken)
    {
        var value = await unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet.AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new QuestionBankRequestDetailsDto
            {
                Id = x.Id, QuestionBankId = x.QuestionBankId,
                RequestTypeId = x.RequestTypeId, RequestTypeNameAr = x.RequestType.NameAr, RequestTypeNameEn = x.RequestType.NameEn,
                StatusId = x.StatusId, StatusNameAr = x.Status.NameAr, StatusNameEn = x.Status.NameEn,
                CurrentReviewRound = x.CurrentReviewRound, Reason = x.Reason,
                SubmittedById = x.SubmittedById, SubmittedByNameAr = x.SubmittedBy.FullNameAr,
                SubmittedByNameEn = x.SubmittedBy.FullNameEn, SubmittedAt = x.SubmittedAt,
                QuestionBankTypeId = x.QuestionBank.QuestionBankTypeId,
                QuestionBankTypeNameAr = x.QuestionBank.QuestionBankType.NameAr,
                QuestionBankTypeNameEn = x.QuestionBank.QuestionBankType.NameEn,
                ManagementId = x.QuestionBank.ManagementId,
                ManagementNameAr = x.QuestionBank.Management == null ? null : x.QuestionBank.Management.NameAr,
                ManagementNameEn = x.QuestionBank.Management == null ? null : x.QuestionBank.Management.NameEn,
                JobTitleId = x.QuestionBank.JobTitleId,
                JobTitleNameAr = x.QuestionBank.JobTitle == null ? null : x.QuestionBank.JobTitle.JobNameAr,
                JobTitleNameEn = x.QuestionBank.JobTitle == null ? null : x.QuestionBank.JobTitle.JobNameEn,
                IsActive = x.QuestionBank.IsActive,
                CanReview = x.StatusId == Tawtheef.Domain.Entities.Lookups.QuestionBankRequestStatusIds.PendingReview &&
                    x.Items.Any(i => !i.IsDeleted && i.RemovedAt == null &&
                                     i.StatusId == Tawtheef.Domain.Entities.Lookups.QuestionBankRequestItemStatusIds.PENDING_REVIEW),
                PendingReviewItemCount = x.Items.Count(i => !i.IsDeleted && i.StatusId == Tawtheef.Domain.Entities.Lookups.QuestionBankRequestItemStatusIds.PENDING_REVIEW),
                ApprovedItemCount = x.Items.Count(i => !i.IsDeleted && i.StatusId == Tawtheef.Domain.Entities.Lookups.QuestionBankRequestItemStatusIds.APPROVED),
                NeedsModificationItemCount = x.Items.Count(i => !i.IsDeleted && i.StatusId == Tawtheef.Domain.Entities.Lookups.QuestionBankRequestItemStatusIds.NEEDS_MODIFICATION),
                RejectedItemCount = x.Items.Count(i => !i.IsDeleted && i.StatusId == Tawtheef.Domain.Entities.Lookups.QuestionBankRequestItemStatusIds.REJECTED),
                Assignments = x.Assignments.OrderBy(a => a.AssignedAt).Select(a => new QuestionBankAssignmentDto
                {
                    Id = a.Id, EmployeeId = a.EmployeeId, EmployeeNameAr = a.Employee.FullNameAr,
                    EmployeeNameEn = a.Employee.FullNameEn, StatusId = a.StatusId,
                    StatusNameAr = a.Status.NameAr, StatusNameEn = a.Status.NameEn,
                    MinimumQuestionCount = a.MinimumQuestionCount, Notes = a.Notes, AssignedAt = a.AssignedAt,
                    QuestionEntryStartedAt = a.QuestionEntryStartedAt, QuestionEntryCompletedAt = a.QuestionEntryCompletedAt,
                    LastReturnedForModificationAt = a.LastReturnedForModificationAt,
                    LastModificationCompletedAt = a.LastModificationCompletedAt, CompletedAt = a.CompletedAt
                }).ToArray()
            }).SingleOrDefaultAsync(cancellationToken);
        return value is null ? Result.Fail<QuestionBankRequestDetailsDto>(ErrorsCodes.QuestionBankRequestNotFound) : Result.Ok(value);
    }
}
