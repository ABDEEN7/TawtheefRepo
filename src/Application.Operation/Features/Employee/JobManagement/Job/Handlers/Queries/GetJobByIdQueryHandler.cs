using Application.Operation.Features.Employee.JobManagement.Job.DTOs;
using Application.Operation.Features.Employee.JobManagement.Job.Queries;
using MediatR;
using FluentResults;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.JobManagement.Job.Handlers.Queries;

public class GetJobByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    : IRequestHandler<GetJobByIdQuery, IResult<JobResponseDto>>
{
    public async Task<IResult<JobResponseDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await uow.GetEntityRepository<Tawtheef.Domain.Entities.Recruitment.Job>().DbSet
            .AsSplitQuery()
            .Include(j => j.JobDegrees).ThenInclude(d => d.Degree)
            .Include(j => j.Department)
            .Include(j => j.JobCategory)
            .Include(j => j.JobTitle)
            .Include(j => j.WorkType)
            .Include(j => j.JobStatus)
            .Include(j => j.Gender)
            .Include(j => j.Major)
            .Include(j => j.SubMajor)
            .Include(j => j.Sector)
            .Include(j => j.Management)
            .Include(j => j.WorkLocation)
            .Include(j => j.JobConditions)
            .Include(j => j.JobPoints).ThenInclude(p => p!.Details)
            .Include(j => j.JobSkills).ThenInclude(s=>s.Skill)
            .Include(j => j.JobResponsibilities)
            .Include(j => j.JobRequiredAttachments)
            .Include(j => j.Invitations).ThenInclude(i => i.History)
            .Include(j => j.TabReviewNotes)
            .Include(j => j.ReviewAttachment)
            .Include(j => j.CandidateFilterSetting).ThenInclude(c => c!.CandidateTypePercentages)
            .Include(j => j.CandidateFilterSetting).ThenInclude(c => c!.NationalityPercentages)
            .Include(j => j.JobSpecializations).ThenInclude(s => s.Major)
            .Include(j => j.JobSpecializations).ThenInclude(s => s.SubMajor)
            .Include(j => j.CreatedBy)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        if (job is null)
            return Result.Fail<JobResponseDto>(JobMessages.JobNotFound);

        var jobDto = mapper.Map<JobResponseDto>(job);
        jobDto.CreatedByName = job.CreatedBy?.FullNameEn ?? "System";
        jobDto.LastActionDate = job.UpdatedDate ?? job.CreatedDate;
        
        return Result.Ok(jobDto);
    }
}

