using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.Job.Commands;
using Application.Operation.Features.Employee.JobManagement.Job.Utilities;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using JobEntity = Tawtheef.Domain.Entities.Recruitment.Job;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Commands;

public sealed class SaveJobPointsCommandHandler(
    IUnitOfWork uow,
    IJobPointsConfigurationsRepository jobPointsConfigurationsRepository
) : IRequestHandler<SaveJobPointsCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(SaveJobPointsCommand cmd, CancellationToken ct)
    {
        var dto = cmd.Request;

        var jobStatus = await uow.GetEntityRepository<JobEntity>()
            .DbSet
            .AsNoTracking()
            .Where(x => x.Id == dto.JobId)
            .Select(x => x.JobStatusId)
            .FirstOrDefaultAsync(ct);

        if (jobStatus == Guid.Empty)
            return Result.Fail<Unit>(JobMessages.JobNotFound);

        if (jobStatus != JobStatusIds.PendingPointConfiguration && jobStatus != JobStatusIds.PendingPointApproval)
            return Result.Fail<Unit>(JobMessages.JobPointsJobNotApproved);

        var configResult = await jobPointsConfigurationsRepository.GetAsync();
        if (configResult.IsFailed)
            return Result.Fail<Unit>(JobMessages.JobPointsConfigurationNotFound);

        var validationResult = JobPointsValidationUtility.Validate(dto, configResult.Value);
        
        if (validationResult.IsFailed)
            return Result.Fail<Unit>(validationResult.Errors);

        // Fetch job to change status
        var jobRepo = uow.GetEntityRepository<JobEntity>();
        var jobResult = await jobRepo.GetByIdAsync(dto.JobId, ct);
        if (jobResult.IsFailed || jobResult.Value is null) return Result.Fail<Unit>(JobMessages.JobNotFound);

        var job = jobResult.Value;
        var repo = uow.GetEntityRepository<JobPointsMain>();

        var main = await repo.DbSet
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.JobId == dto.JobId, ct);

        if (main == null)
        {
            main = new JobPointsMain
            {
                JobId = dto.JobId,
                ApplicantCategory = dto.ApplicantCategory,
                Education = dto.Education,
                Experience = dto.Experience,
                Training = dto.Training,
                Skills = dto.Skills,
                Languages = dto.Languages,
                Certificates = dto.Certificates,
                Total = dto.Total,
                Details = [.. dto.Details.Select(d => new JobPointsDetail
                {
                    Type = d.Type,
                    Code = d.Code,
                    Name = d.Name,
                    ReferenceId = d.ReferenceId,
                    Points = d.Points
                })]
            };

            await repo.AddAsync(main, ct);
        }
        else
        {
            if (main.IsApproved)
                return Result.Fail<Unit>(JobMessages.JobPointsAlreadyApproved);
            
            main.Total = dto.Total;
            main.ApplicantCategory = dto.ApplicantCategory;
            main.Education = dto.Education;
            main.Experience = dto.Experience;
            main.Training = dto.Training;
            main.Skills = dto.Skills;
            main.Languages = dto.Languages;
            main.Certificates = dto.Certificates;

            foreach (var d in dto.Details)
            {
                var detail = main.Details
                    .FirstOrDefault(x => x.Code == d.Code && x.Type == d.Type);

                if (detail == null)
                {
                    main.Details.Add(new JobPointsDetail
                    {
                        Type = d.Type,
                        Code = d.Code,
                        Name = d.Name,
                        ReferenceId = d.ReferenceId,
                        Points = d.Points
                    });
                    continue;
                }

                detail.Points = d.Points;
                detail.Name = d.Name;
                detail.ReferenceId = d.ReferenceId;
            }
        }

        job.ChangeStatus(JobStatusIds.PendingPointApproval);
        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}

