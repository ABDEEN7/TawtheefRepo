using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.Job.DTOs;
using Application.Operation.Features.Employee.Job.Queries;
using Cortex.Mediator.Queries;
using FluentResults;
using MapsterMapper;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Services;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.Job.Handlers.Queries;

public class GetJobCopyTemplateQueryHandler(
    IJobRepository jobRepository,
    IJobPointsRepository jobPointsRepository,
    IMapper mapper)
    : IQueryHandler<GetJobCopyTemplateQuery, IResult<JobCopyTemplateDto>>
{
    public async Task<IResult<JobCopyTemplateDto>> Handle(
        GetJobCopyTemplateQuery request,
        CancellationToken cancellationToken)
    {
        var jobResult = await jobRepository.GetByIdWithDetailsAsync(request.JobId);
        if (jobResult.IsFailed || jobResult.Value == null)
            return Result.Fail<JobCopyTemplateDto>(JobMessages.JobNotFound);

        var job = jobResult.Value;
        if (!JobBusinessRules.CanCopyFromPreviousJob(job.JobStatusId))
            return Result.Fail<JobCopyTemplateDto>(JobMessages.JobCannotBeCopied);

        var template = mapper.Map<JobCopyTemplateDto>(job);
        template.NumberOfVacancies = null;
        template.ClosingDate = null;

        if (job.JobPoints != null)
        {
            var jobPointsResult = await jobPointsRepository.GetByJobIdAsync(job.Id);
            if (jobPointsResult.IsSuccess)
            {
                template.JobPoints = mapper.Map<JobPointsCopyDto>(jobPointsResult.Value);
            }
        }

        return Result.Ok(template);
    }
}
