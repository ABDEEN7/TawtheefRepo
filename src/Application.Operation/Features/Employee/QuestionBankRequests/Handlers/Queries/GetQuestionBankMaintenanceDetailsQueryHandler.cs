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
        var details = await unitOfWork.GetEntityRepository<QuestionBank>().DbSet.AsNoTracking()
            .Where(x => x.Id == request.QuestionBankId && !x.IsDeleted)
            .ProjectToType<QuestionBankMaintenanceDetailsDto>(mapper.Config)
            .SingleOrDefaultAsync(cancellationToken);

        return details is null
            ? Result.Fail<QuestionBankMaintenanceDetailsDto>(ErrorsCodes.QuestionBankNotFound)
            : Result.Ok(details);
    }
}
