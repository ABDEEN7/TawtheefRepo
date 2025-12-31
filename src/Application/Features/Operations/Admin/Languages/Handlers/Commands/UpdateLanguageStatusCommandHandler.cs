using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Features.Operations.Admin.Languages.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Operations.Admin.Languages.Handlers.Commands;

public sealed class UpdateLanguageStatusCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateLanguageStatusCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(
        UpdateLanguageStatusCommand request,
        CancellationToken cancellationToken)
    {
        var repository = unitOfWork.GetEntityRepository<Language>();
        var language = await repository.DbSet.FirstOrDefaultAsync(l => l.Id == request.LanguageId, cancellationToken);

        if (language is null)
            return Result.Fail<Unit>(ErrorsCodes.LanguageNotFound);

        var now = timeProvider.GetUtcNow();
        var hasUser = Guid.TryParse(currentUserService.UserId, out var userId);

        language.IsActive = request.IsActive;
        language.UpdatedDate = now;
        language.UpdatedById = hasUser ? userId : language.UpdatedById;

        await repository.UpdateAsync(language);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(Unit.Value);
    }
}
