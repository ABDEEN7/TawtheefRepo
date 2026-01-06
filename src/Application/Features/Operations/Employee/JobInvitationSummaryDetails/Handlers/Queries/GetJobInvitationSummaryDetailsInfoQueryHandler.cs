using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsInfoQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILocalizationService localizationService)
    : IQueryHandler<GetJobInvitationSummaryDetailsInfoQuery, IResult<JobInvitationSummaryDetailsInfoDto>>
{
    public async Task<IResult<JobInvitationSummaryDetailsInfoDto>> Handle(
        GetJobInvitationSummaryDetailsInfoQuery query,
        CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Domain.Entities.Recruitment.Job>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == query.JobId, cancellationToken);

        if (job is null)
            return Result.Fail<JobInvitationSummaryDetailsInfoDto>(JobMessages.JobNotFound);

        var dto = mapper.Map<JobInvitationSummaryDetailsInfoDto>(job);
        dto.JobName = localizationService.GetLocalizedValue(job.TitleAr, job.TitleEn);

        return Result.Ok(dto);
    }
}
