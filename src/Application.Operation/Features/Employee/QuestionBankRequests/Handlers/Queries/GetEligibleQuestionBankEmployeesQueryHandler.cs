using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using Application.Operation.Features.Employee.QuestionBankRequests.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Queries;

public sealed class GetEligibleQuestionBankEmployeesQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetEligibleQuestionBankEmployeesQuery, IResult<IReadOnlyCollection<QuestionBankEmployeeLookupDto>>>
{
    public async Task<IResult<IReadOnlyCollection<QuestionBankEmployeeLookupDto>>> Handle(GetEligibleQuestionBankEmployeesQuery request, CancellationToken cancellationToken)
    {
        var target = await unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet.AsNoTracking()
            .Where(x => x.Id == request.RequestId)
            .Select(x => new { x.QuestionBank.QuestionBankTypeId, x.QuestionBank.ManagementId })
            .SingleOrDefaultAsync(cancellationToken);
        if (target is null) return Result.Fail<IReadOnlyCollection<QuestionBankEmployeeLookupDto>>(ErrorsCodes.QuestionBankRequestNotFound);

        // EmployeeProfile currently carries the directory's stable ORGNO in DepartmentNumber.
        // Department.BackendName is the organization lookup's stable external code; display names are never compared.
        if (target.QuestionBankTypeId != QuestionBankTypeIds.Specialized || target.ManagementId is null)
            return Result.Fail<IReadOnlyCollection<QuestionBankEmployeeLookupDto>>(ErrorsCodes.QuestionBankEmployeeScopeNotConfigured);

        var departmentCodes = unitOfWork.Context.Set<Department>().AsNoTracking()
            .Where(x => x.ManagementId == target.ManagementId).Select(x => x.BackendName);
        var search = request.Search?.Trim();
        var employees = await unitOfWork.Context.Set<EmployeeUser>().AsNoTracking()
            .Where(x => !x.IsDeleted && !x.IsBlocked && x.EmployeeProfile != null &&
                        !x.EmployeeProfile.IsDeleted && x.EmployeeProfile.DepartmentNumber != null &&
                        departmentCodes.Contains(x.EmployeeProfile.DepartmentNumber))
            .Where(x => string.IsNullOrEmpty(search) || x.FullNameAr.Contains(search) || x.FullNameEn.Contains(search))
            .OrderBy(x => x.FullNameEn).Take(50)
            .Select(x => new QuestionBankEmployeeLookupDto(x.Id, x.FullNameAr, x.FullNameEn))
            .ToArrayAsync(cancellationToken);
        return Result.Ok<IReadOnlyCollection<QuestionBankEmployeeLookupDto>>(employees);
    }
}
