using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Dashboard.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Tawtheef.Application.Features.Recruitment.Dashboard.Handlers.Commands;

public sealed class ChangeStatusCandidateInvitationCommandHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<ChangeStatusCandidateInvitationCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(ChangeStatusCandidateInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .FirstOrDefaultAsync(
                inv => inv.Id == command.InvitationId && inv.ApplicantId == command.UserId,
                cancellationToken);

        if (invitation is null)
            return Result.Fail<Unit>(ErrorsCodes.InvitationNotFound);

        invitation.ChangeInvitationStatus(InvitationStatusIds.Read);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
