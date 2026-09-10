using Application.Operation.Features.Interview.Committee.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Interview.Committee.Handlers.Commands;

public sealed class CreateCommitteeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCommitteeCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateCommitteeCommand request, CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Job>().DbSet
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);
        if (job is null)
            return Result.Fail<Guid>(new Error(ErrorsCodes.JobNotFound));

        if (job.JobStatusId != JobStatusIds.Published)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewCommitteeJobNotPublished));

        var templateExists = await unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet
            .AnyAsync(t => t.Id == request.InterviewTemplateId, cancellationToken);
        if (!templateExists)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewTemplateNotFound));

        var committeeRepo = unitOfWork.GetEntityRepository<InterviewCommittee>();

        var jobAlreadyHasActiveCommittee = await committeeRepo.DbSet
            .AnyAsync(c => c.JobId == request.JobId && c.IsActive, cancellationToken);
        if (jobAlreadyHasActiveCommittee)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewCommitteeAlreadyExistsForJob));

        var committee = InterviewCommittee.Create(
            request.JobId,
            request.InterviewTemplateId,
            request.NameAr,
            request.NameEn,
            request.ScopeDescription,
            request.Notes);

        await committeeRepo.AddAsync(committee);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(committee.Id);
    }
}
