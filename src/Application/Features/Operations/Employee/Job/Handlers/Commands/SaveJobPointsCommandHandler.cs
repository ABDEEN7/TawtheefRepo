using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.Job.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Handlers.Commands;

public sealed class SaveJobPointsCommandHandler(
    IUnitOfWork uow
) : IRequestHandler<SaveJobPointsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveJobPointsCommand cmd, CancellationToken ct)
    {
        var jobRepo = uow.GetEntityRepository<JobEntity>();

        var job = await jobRepo.DbSet
            .Include(j => j.JobPoints)
                .ThenInclude(p => p!.Details)
            .FirstOrDefaultAsync(j => j.Id == cmd.Request.JobId, ct);

        if (job is null)
            return Result.Fail<Unit>(JobMessages.JOB_NOT_FOUND);

        var dto = cmd.Request;

        var jobPointsMain = job.JobPoints ?? new JobPointsMain
        {
            JobId = job.Id,
            Details = new List<JobPointsDetail>()
        };

        jobPointsMain.ApplicantCategory = dto.ApplicantCategory;
        jobPointsMain.Education = dto.Education;
        jobPointsMain.Experience = dto.Experience;
        jobPointsMain.Training = dto.Training;
        jobPointsMain.Skills = dto.Skills;
        jobPointsMain.Languages = dto.Languages;
        jobPointsMain.Certificates = dto.Certificates;
        jobPointsMain.Total = dto.Total;

        foreach (var detailDto in dto.Details)
        {
            var existingDetail = jobPointsMain.Details?
                .FirstOrDefault(d => d.Code == detailDto.Code && d.Type == detailDto.Type);

            if (existingDetail != null)
            {
                existingDetail.Points = detailDto.Points;
                existingDetail.Name = detailDto.Name;
                existingDetail.ReferenceId = detailDto.ReferenceId;
                existingDetail.IsDeleted = false;
            }
            else
            {
                
                jobPointsMain.Details?.Add(new JobPointsDetail
                {
                    JobPointsMain = jobPointsMain,
                    Type = detailDto.Type,
                    Code = detailDto.Code,
                    Name = detailDto.Name,
                    Points = detailDto.Points,
                    ReferenceId = detailDto.ReferenceId,
                    IsDeleted = false
                });
            }
        }

        job.JobPoints ??= jobPointsMain;

        await uow.SaveChangesAsync(ct);

        return Result.Ok(Unit.Value);
    }
}
