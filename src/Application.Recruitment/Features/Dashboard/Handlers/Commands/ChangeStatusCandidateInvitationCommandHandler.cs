using Application.Recruitment.Features.Dashboard.Commands;
using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Recruitment.Features.Dashboard.Handlers.Commands;

public sealed class ChangeStatusCandidateInvitationRejectedCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<ChangeStatusCandidateInvitationRejectedCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeStatusCandidateInvitationRejectedCommand command, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .FirstOrDefaultAsync(
                inv => inv.Id == command.InvitationId && inv.ApplicantId == command.UserId,
                cancellationToken);

        if (invitation is null)
            return Result.Fail<Unit>(ErrorsCodes.InvitationNotFound);

        var isPending = invitation.InvitationStatusId == InvitationStatusIds.NewInvitation
            || invitation.InvitationStatusId == InvitationStatusIds.Read;

        if (!isPending)
            return Result.Fail<Unit>(ErrorsCodes.InvitationStatusChangeNotAllowed);

        invitation.ChangeInvitationStatus(InvitationStatusIds.Rejected);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
