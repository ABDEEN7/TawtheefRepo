using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using Application.Operation.Features.Employee.QuestionBankRequests.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Queries;

public sealed class GetQuestionBankMaintenanceDetailsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetQuestionBankMaintenanceDetailsQuery, IResult<QuestionBankMaintenanceDetailsDto>>
{
    public async Task<IResult<QuestionBankMaintenanceDetailsDto>> Handle(
        GetQuestionBankMaintenanceDetailsQuery request, CancellationToken cancellationToken)
    {
        var details = await unitOfWork.GetEntityRepository<QuestionBank>().DbSet.AsNoTracking()
            .Where(x => x.Id == request.QuestionBankId && !x.IsDeleted)
            .Select(x => new QuestionBankMaintenanceDetailsDto
            {
                QuestionBankId = x.Id,
                QuestionBankTypeId = x.QuestionBankTypeId,
                QuestionBankTypeNameAr = x.QuestionBankType.NameAr,
                QuestionBankTypeNameEn = x.QuestionBankType.NameEn,
                ManagementId = x.ManagementId,
                ManagementNameAr = x.Management == null ? null : x.Management.NameAr,
                ManagementNameEn = x.Management == null ? null : x.Management.NameEn,
                JobTitleId = x.JobTitleId,
                JobTitleNameAr = x.JobTitle == null ? null : x.JobTitle.JobNameAr,
                JobTitleNameEn = x.JobTitle == null ? null : x.JobTitle.JobNameEn,
                CurrentApprovedVersionId = x.CurrentApprovedVersionId,
                CurrentVersionNo = x.CurrentApprovedVersion == null ? null : x.CurrentApprovedVersion.VersionNo,
                IsActive = x.IsActive
            }).SingleOrDefaultAsync(cancellationToken);

        return details is null
            ? Result.Fail<QuestionBankMaintenanceDetailsDto>(ErrorsCodes.QuestionBankNotFound)
            : Result.Ok(details);
    }
}
