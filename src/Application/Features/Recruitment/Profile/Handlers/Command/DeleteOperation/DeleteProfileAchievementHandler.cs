using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.Command.DeleteOperations;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Applicant;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command.DeleteOperation;

public sealed class DeleteProfileAchievementHandler(IUnitOfWork uow)
    : IRequestHandler<DeleteProfileAchievementCommand, IResult<Unit>>
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
