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
        var dto = cmd.Request;

        if (!await uow.GetEntityRepository<JobEntity>()
                .DbSet.AnyAsync(x => x.Id == dto.JobId, ct))
            return Result.Fail<Unit>(JobMessages.JobNotFound);

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

            await repo.AddAsync(main);
        }
        else
        {
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

                detail?.Points = d.Points;
            }
        }

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
