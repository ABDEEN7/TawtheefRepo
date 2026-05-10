using Application.Operation.Features.Admin.ProfileLogs;
using Application.Operation.Features.Admin.ProfileLogs.DTOs;
using Application.Operation.Features.Employee.CandidateUsers.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.CandidateUsers.Handlers.Queries;

public sealed class GetCandidateUserProfileLogsHandler(
    IUnitOfWork uow,
    UserManager<User> userManager)
    : IRequestHandler<GetCandidateUserProfileLogsQuery, IResult<PaginatedResult<ProfileLogDto>>>
{
    public async Task<IResult<PaginatedResult<ProfileLogDto>>> Handle(
        GetCandidateUserProfileLogsQuery request,
        CancellationToken cancellationToken)
    {
        var profileId = await uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(p => p.UserId == request.UserId)
            .Select(p => (Guid?)p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!profileId.HasValue)
        {
            var empty = new PaginatedResult<ProfileLogDto>([], 0, request.PageNumber, request.PageSize);
            return Result.Ok(empty);
        }

        var loggerRepo = uow.GetEntityRepository<UserProfileLogger>();
        var auditRepo = uow.GetEntityRepository<AuditTrailEntry>();

        var logsQuery = loggerRepo.DbSet
            .AsNoTracking()
            .Where(x => x.UserProfileId == profileId.Value)
            .Select(log => new ProfileLogProjection
            {
                Id = log.Id,
                UserProfileId = log.UserProfileId,
                UserId = log.PerformedById ?? log.CreatedById,
                ActionType = log.ActionType,
                Section = log.Section,
                Notes = log.Notes,
                ReviewStatus = log.ReviewStatus,
                EntityId = log.EntityId,
                AttachmentId = log.AttachmentId,
                Source = ProfileLogSources.UserProfileLogger,
                CreatedDate = log.CreatedDate
            })
            .Concat(auditRepo.DbSet
                .AsNoTracking()
                .Where(x => x.UserProfileId == profileId.Value)
                .Select(log => new ProfileLogProjection
                {
                    Id = log.Id,
                    UserProfileId = log.UserProfileId,
                    UserId = log.UserId,
                    ActionType = log.ActionType,
                    Section = log.Section,
                    Notes = log.Notes,
                    ReviewStatus = null,
                    EntityId = log.EntityId,
                    AttachmentId = log.AttachmentId,
                    Source = ProfileLogSources.AuditTrail,
                    CreatedDate = log.CreatedDate
                }));

        var totalCount = await logsQuery.CountAsync(cancellationToken);
        var items = await logsQuery
            .OrderByDescending(x => x.CreatedDate)
            .ThenByDescending(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userIds = items
            .Where(x => x.UserId.HasValue)
            .Select(x => x.UserId!.Value)
            .Distinct()
            .ToArray();

        var users = userIds.Length == 0
            ? []
            : await userManager.Users
                .AsNoTracking()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullNameAr, u.FullNameEn, u.Email })
                .ToListAsync(cancellationToken);

        var userLookup = users.ToDictionary(
            u => u.Id,
            u => !string.IsNullOrWhiteSpace(u.FullNameAr) ? u.FullNameAr : (!string.IsNullOrWhiteSpace(u.FullNameEn) ? u.FullNameEn : u.Email ?? string.Empty));

        var ownerName = await uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(p => p.Id == profileId.Value)
            .Select(p => !string.IsNullOrWhiteSpace(p.User!.FullNameAr) ? p.User!.FullNameAr : (!string.IsNullOrWhiteSpace(p.User!.FullNameEn) ? p.User!.FullNameEn : p.User!.Email ?? string.Empty))
            .FirstOrDefaultAsync(cancellationToken);

        var mapped = items.Select(log => new ProfileLogDto
        {
            Id = log.Id,
            UserProfileId = log.UserProfileId,
            Source = log.Source,
            ActionType = log.ActionType,
            Section = log.Section,
            Notes = log.Notes,
            UserId = log.UserId,
            UserName = log.UserId.HasValue && userLookup.TryGetValue(log.UserId.Value, out var name) ? name : null,
            UserProfileOwnerName = ownerName,
            EntityId = log.EntityId,
            AttachmentId = log.AttachmentId,
            ReviewStatus = log.ReviewStatus,
            CreatedDate = log.CreatedDate
        }).ToList();

        var result = new PaginatedResult<ProfileLogDto>(mapped, totalCount, request.PageNumber, request.PageSize);
        return Result.Ok(result);
    }

    private sealed record ProfileLogProjection
    {
        public required Guid Id { get; init; }
        public required Guid UserProfileId { get; init; }
        public Guid? UserId { get; init; }
        public required string ActionType { get; init; }
        public string? Section { get; init; }
        public string? Notes { get; init; }
        public ReviewStatus? ReviewStatus { get; init; }
        public Guid? EntityId { get; init; }
        public Guid? AttachmentId { get; init; }
        public required string Source { get; init; }
        public required DateTimeOffset CreatedDate { get; init; }
    }
}

