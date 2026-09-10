using Application.Operation.Features.Interview.EvaluationTemplate.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Interview;

namespace Application.Operation.Features.Interview.EvaluationTemplate.Handlers.Commands;

public sealed class CreateTemplateVersionCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTemplateVersionCommand, IResult<Guid>>
{
    public async Task<IResult<Guid>> Handle(CreateTemplateVersionCommand request, CancellationToken cancellationToken)
    {
        var templateExists = await unitOfWork.GetEntityRepository<InterviewTemplate>().DbSet
            .AnyAsync(t => t.Id == request.InterviewTemplateId, cancellationToken);
        if (!templateExists)
            return Result.Fail<Guid>(new Error(ErrorsCodes.InterviewTemplateNotFound));

        var versionRepo = unitOfWork.GetEntityRepository<InterviewTemplateVersion>();

        var lastVersionNo = await versionRepo.DbSet
            .Where(v => v.InterviewTemplateId == request.InterviewTemplateId)
            .Select(v => (int?)v.VersionNo)
            .MaxAsync(cancellationToken) ?? 0;

        var version = InterviewTemplateVersion.Create(
            request.InterviewTemplateId,
            lastVersionNo + 1,
            request.FinalScore,
            request.QualificationScore,
            request.CalculationMethod);

        await versionRepo.AddAsync(version);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(version.Id);
    }
}
