using Application.Operation.Features.Employee.Exams.DTOs;
using Application.Operation.Features.Employee.Exams.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.Exams.Handlers.Queries;

public sealed class GetExamLookupsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetExamLookupsQuery, IResult<ExamLookupsDto>>
{
    public async Task<IResult<ExamLookupsDto>> Handle(GetExamLookupsQuery request, CancellationToken ct)
    {
        var categories = await unitOfWork.Context.Set<QuestionBankType>().AsNoTracking()
            .Select(x => new ExamLookupItemDto(x.Id, x.NameAr, x.NameEn)).ToListAsync(ct);
        var policies = await unitOfWork.Context.Set<ExamInterruptionPolicy>().AsNoTracking()
            .Select(x => new ExamLookupItemDto(x.Id, x.NameAr, x.NameEn)).ToListAsync(ct);
        return Result.Ok(new ExamLookupsDto(categories, policies, QuestionBankTypeIds.Specialized));
    }
}
