using Application.Operation.Features.Employee.QuestionBankRequests;
using Application.Operation.Features.Employee.QuestionBankRequests.DTOs;
using Application.Operation.Features.Employee.QuestionBankRequests.Queries;
using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankRequests.Handlers.Queries;

public sealed class GetQuestionBankMaintenanceDetailsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetQuestionBankMaintenanceDetailsQuery, IResult<QuestionBankMaintenanceDetailsDto>>
{
    public async Task<IResult<QuestionBankMaintenanceDetailsDto>> Handle(
        GetQuestionBankMaintenanceDetailsQuery request, CancellationToken cancellationToken)
    {
        var bank = await unitOfWork.GetEntityRepository<QuestionBank>().DbSet.AsNoTracking()
            .Include(x => x.CurrentApprovedVersion)
            .SingleOrDefaultAsync(x => x.Id == request.QuestionBankId && !x.IsDeleted, cancellationToken);

        if (bank is null)
            return Result.Fail<QuestionBankMaintenanceDetailsDto>(ErrorsCodes.QuestionBankNotFound);

        var maintainabilityError = QuestionBankMaintenanceRules.GetMaintainabilityError(bank);
        if (maintainabilityError is not null)
            return Result.Fail<QuestionBankMaintenanceDetailsDto>(maintainabilityError);

        var hasOpenRequest = await unitOfWork.GetEntityRepository<QuestionBankRequest>().DbSet
            .AsNoTracking()
            .AnyAsync(x => !x.IsDeleted && x.QuestionBankId == bank.Id &&
                QuestionBankMaintenanceRules.OpenStatuses.Contains(x.StatusId), cancellationToken);
        if (hasOpenRequest)
        {
            return Result.Fail<QuestionBankMaintenanceDetailsDto>(
                new Error(ErrorsCodes.QuestionBankActiveRequestAlreadyExists)
                    .WithMetadata("Code", "Conflict")
                    .WithMetadata("UserMessage", "There is already an active question bank request for this bank."));
        }

        var details = await unitOfWork.GetEntityRepository<QuestionBank>().DbSet.AsNoTracking()
            .Where(x => x.Id == bank.Id)
            .ProjectToType<QuestionBankMaintenanceDetailsDto>(mapper.Config)
            .SingleAsync(cancellationToken);
        return Result.Ok(details);
    }
}
