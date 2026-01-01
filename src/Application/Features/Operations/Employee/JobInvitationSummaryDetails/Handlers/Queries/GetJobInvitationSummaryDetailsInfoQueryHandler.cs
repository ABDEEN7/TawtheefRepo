using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.DTOs;
using Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Queries;
using Tawtheef.Domain.Constants;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;
namespace Tawtheef.Application.Features.Operations.Employee.JobInvitationSummaryDetails.Handlers.Queries;

public sealed class GetJobInvitationSummaryDetailsInfoQueryHandler(
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetJobInvitationSummaryDetailsInfoQuery, IResult<JobInvitationSummaryDetailsInfoDto>>
{
    public async Task<IResult<JobInvitationSummaryDetailsInfoDto>> Handle(
        GetJobInvitationSummaryDetailsInfoQuery query,
        CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<JobEntity>().DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == query.JobId, cancellationToken);

        if (job is null)
            return Result.Fail<JobInvitationSummaryDetailsInfoDto>(JobMessages.JobNotFound);

        var language = httpContextAccessor.HttpContext?.Items["Language"]?.ToString()?.ToLower();
        var jobName = string.Equals(language, "ar", StringComparison.OrdinalIgnoreCase)
            ? job.TitleAr
            : job.TitleEn;

        return Result.Ok(new JobInvitationSummaryDetailsInfoDto
        {
            JobId = job.Id,
            JobName = jobName
        });
    }
}
