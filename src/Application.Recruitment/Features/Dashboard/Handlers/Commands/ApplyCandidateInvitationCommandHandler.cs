using Application.Recruitment.Features.Dashboard.Commands;
using MediatR;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Dashboard.Handlers.Commands;

public sealed class ApplyCandidateInvitationCommandHandler(IUnitOfWork unitOfWork, TimeProvider timeProvider)
    : IRequestHandler<ApplyCandidateInvitationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ApplyCandidateInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .FirstOrDefaultAsync(
                inv => inv.Id == command.InvitationId && inv.ApplicantId == command.UserId,
                cancellationToken);

        if (invitation is null)
            return Result.Fail<Unit>(ErrorsCodes.InvitationNotFound);

        var canSubmit = invitation.InvitationStatusId == InvitationStatusIds.NewInvitation
            || invitation.InvitationStatusId == InvitationStatusIds.Read;

        if (!canSubmit)
            return Result.Fail<Unit>(ErrorsCodes.InvitationStatusChangeNotAllowed);

        invitation.ChangeInvitationStatus(InvitationStatusIds.Submitted);
        invitation.IsAccepted = true;
        invitation.AcceptedAt = timeProvider.GetUtcNow().UtcDateTime;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}

