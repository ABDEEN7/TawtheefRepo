using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Queries;

public sealed class GetMyUserProfileSummaryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMyUserProfileSummaryQuery, Result<UserProfileSummaryDto>>
{
    public async Task<Result<UserProfileSummaryDto>> Handle(GetMyUserProfileSummaryQuery request, CancellationToken ct)
    {
        var profileResult = await UserProfileLoader.GetSummaryAsync(uow, request.UserId, ct);
        if (profileResult.IsFailed) return Result.Fail<UserProfileSummaryDto>(profileResult.Errors);
        var profile = profileResult.Value;
        if (profile is null) return Result.Fail<UserProfileSummaryDto>(ErrorsCodes.UserProfileNotFound);
        
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
