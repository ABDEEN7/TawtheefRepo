using Application.Operation.Features.Admin.ProfileLogs.DTOs;
using Application.Operation.Features.Admin.ProfileLogs.Queries;
using Application.Operation.Features.Employee.CandidateUsers.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Handlers.Queries;

public sealed class GetCandidateUserProfileLogsHandler(
    IUnitOfWork uow,
    ISender sender)
    : IRequestHandler<GetCandidateUserProfileLogsQuery, IResult<PaginatedResult<ProfileLogDto>>>
{
    public async Task<IResult<PaginatedResult<ProfileLogDto>>> Handle(
        GetCandidateUserProfileLogsQuery request,
        CancellationToken cancellationToken)
    {
        var profileId = await uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(profile => profile.UserId == request.UserId)
            .Select(profile => (Guid?)profile.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!profileId.HasValue)
        {
            return Result.Ok(
                new PaginatedResult<ProfileLogDto>([], 0, request.PageNumber, request.PageSize));
        }

        return await sender.Send(
            new GetProfileLogsQuery
            {
                UserProfileId = profileId.Value.ToString(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            },
            cancellationToken);
    }
}
