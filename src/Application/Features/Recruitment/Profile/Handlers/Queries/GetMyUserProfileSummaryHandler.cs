using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.Command;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Command;

public sealed class GetMyUserProfileSummaryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMyUserProfileSummaryQuery, Result<UserProfileSummaryDto>>
{
    public async Task<Result<UserProfileSummaryDto>> Handle(GetMyUserProfileSummaryQuery request, CancellationToken ct)
    {
        var profile = await UserProfileLoader.GetOrCreateAsync(uow, request.UserId, ct);

        var canEdit = profile.Status == UserProfileStatus.InCreation;
        var canSubmit = canEdit && profile.IsCompleted();

        return Result.Ok(new UserProfileSummaryDto
        {
            UserProfileId = profile.Id,
            Status = profile.Status,
            CanEdit = canEdit,
            CanSubmit = canSubmit
        });
    }
}
