using Application.Operation.Features.Employee.MinisterOffice.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.MinisterOffice;

namespace Application.Operation.Features.Employee.MinisterOffice.Handlers;

public sealed class UpdateMinisterOfficeCandidateFollowUpStatusCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateMinisterOfficeCandidateFollowUpStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateMinisterOfficeCandidateFollowUpStatusCommand request,
        CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<MinisterOfficeCandidate>();
        var candidate = await repo.DbSet
            .FirstOrDefaultAsync(c => c.Id == request.CandidateId, ct);

        if (candidate is null)
            return Result.Fail<Unit>(ErrorsCodes.MinisterOfficeCandidateNotFound);

        candidate.IsFollowUpActive = request.IsActive;

        var action = request.IsActive
            ? MinisterOfficeCandidateAuditActions.FollowUpReactivated
            : MinisterOfficeCandidateAuditActions.FollowUpDeactivated;

        var auditRepo = uow.GetEntityRepository<MinisterOfficeCandidateAuditLog>();
        await auditRepo.AddAsync(new MinisterOfficeCandidateAuditLog
        {
            CandidateId = candidate.Id,
            Qid = candidate.Qid,
            Action = action,
            Details = $"Follow-up set to {(request.IsActive ? "Active" : "Inactive")}"
        }, ct);

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
