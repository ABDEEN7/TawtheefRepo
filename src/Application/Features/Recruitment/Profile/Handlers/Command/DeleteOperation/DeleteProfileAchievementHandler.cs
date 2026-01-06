using Cortex.Mediator;
using Cortex.Mediator.Commands;
using FluentResults;

using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperation;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.DeleteOperation;

public sealed class DeleteProfileAchievementHandler(IUnitOfWork uow)
    : ICommandHandler<DeleteProfileAchievementCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(DeleteProfileAchievementCommand request, CancellationToken cancellationToken)
    {
        var repo = uow.GetEntityRepository<Achievement>();
        var achievement = await repo.GetByIdAsync(request.AchievementId);
        if (achievement.IsFailed || achievement.Value is null)
            return Result.Fail<Unit>(ErrorsCodes.AchievementNotFound);

        await repo.DeleteAsync(request.AchievementId);
        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
