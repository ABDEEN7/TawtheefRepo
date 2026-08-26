using Application.Operation.Features.Admin.ProfileLogs;
using Application.Operation.Features.Admin.SystemAdminLogs.DTOs;
using Application.Operation.Features.Admin.SystemAdminLogs.Queries;
using MediatR;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Logger;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Admin.SystemAdminLogs.Handlers.Queries;

public sealed class GetSystemAdminLogsQueryHandler(
    IUnitOfWork uow,
    UserManager<User> userManager,
    ILocalizationService localizationService)
    : IRequestHandler<GetSystemAdminLogsQuery, IResult<PaginatedResult<SystemAdminLogDto>>>
{
    public async Task<IResult<PaginatedResult<SystemAdminLogDto>>> Handle(
        GetSystemAdminLogsQuery request,
        CancellationToken cancellationToken)
    {
        var actionLogRepo = uow.GetEntityRepository<ActionLog>();
        var profileLogRepo = uow.GetEntityRepository<UserProfileLogger>();

        // 1. Action Logs (Admin actions)
        var actionLogs = actionLogRepo.DbSet
            .Select(log => new SystemAdminLogProjection
            {
                Id = log.Id,
                UserProfileId = log.UserProfileId ?? Guid.Empty,
                UserId = log.UserId ?? Guid.Empty,
                ActionType = log.ActionType,
                Section = log.Section,
                Notes = log.Notes,
                EntityId = log.EntityId,
                AttachmentId = log.AttachmentId,
                ReviewStatus = null,
                LogType = log.LogType.ToString(),
                Source = ProfileLogSources.ActionLog,
                CreatedDate = log.CreatedDate
            });

        // 2. Profile Logs (Specific profile activities)
        var profileLogs = profileLogRepo.DbSet
            .Select(log => new SystemAdminLogProjection
            {
                Id = log.Id,
                UserProfileId = log.UserProfileId,
                UserId = log.CreatedById ?? Guid.Empty,
                ActionType = log.ActionType,
                Section = "Profile",
                Notes = log.Notes,
                EntityId = null,
                AttachmentId = null,
                ReviewStatus = log.ReviewStatus,
                LogType = null,
                Source = ProfileLogSources.UserProfileLogger,
                CreatedDate = log.CreatedDate
            });

        // 3. Combined Query
        var query = actionLogs.Union(profileLogs);

        // 4. Apply Filters
        var actionType = request.ActionType?.Trim();
        query = query
            .WhereIf(request.UserProfileId.HasValue, log => log.UserProfileId == request.UserProfileId!.Value)
            .WhereIf(request.UserId.HasValue, log => log.UserId == request.UserId!.Value)
            .WhereIf(
                !string.IsNullOrWhiteSpace(actionType),
                log => EF.Functions.Like(log.ActionType, $"%{actionType}%"))
            .WhereIf(request.From.HasValue, log => log.CreatedDate >= request.From!.Value)
            .WhereIf(request.To.HasValue, log => log.CreatedDate <= request.To!.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(log =>
                EF.Functions.Like(log.ActionType, $"%{search}%") ||
                (log.Section != null && EF.Functions.Like(log.Section, $"%{search}%")) ||
                (log.Notes != null && EF.Functions.Like(log.Notes, $"%{search}%")));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(log => log.CreatedDate)
            .ThenByDescending(log => log.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var userLookup = await BuildUserLookupAsync(items, cancellationToken);
        var profileOwnerLookup = await BuildProfileOwnerLookupAsync(items, cancellationToken);

        var mapped = items
            .Select(log => new SystemAdminLogDto
            {
                Id = log.Id,
                UserProfileId = log.UserProfileId,
                UserId = log.UserId,
                UserName = userLookup.GetValueOrDefault(log.UserId),
                UserProfileOwnerName = profileOwnerLookup.GetValueOrDefault(log.UserProfileId),
                Source = log.Source,
                ActionType = log.ActionType,
                Section = log.Section,
                Notes = log.Notes,
                EntityId = log.EntityId,
                AttachmentId = log.AttachmentId,
                ReviewStatus = log.ReviewStatus,
                LogType = log.LogType,
                CreatedDate = log.CreatedDate
            })
            .ToList();

        return Result.Ok(new PaginatedResult<SystemAdminLogDto>(mapped, totalCount, request.PageNumber, request.PageSize));
    }

    private async Task<Dictionary<Guid, string>> BuildUserLookupAsync(
        IEnumerable<SystemAdminLogProjection> logs,
        CancellationToken cancellationToken)
    {
        var userIds = logs
            .Select(l => l.UserId)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        if (userIds.Length == 0)
            return new Dictionary<Guid, string>();

        var users = await userManager.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.FullNameAr, u.FullNameEn, u.Email })
            .ToListAsync(cancellationToken);

        return users.ToDictionary(
            u => u.Id,
            u => GetLocalizedName(u.FullNameAr, u.FullNameEn, u.Email));
    }

    private async Task<Dictionary<Guid, string>> BuildProfileOwnerLookupAsync(
        IEnumerable<SystemAdminLogProjection> logs,
        CancellationToken cancellationToken)
    {
        var profileIds = logs
            .Select(l => l.UserProfileId)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        if (profileIds.Length == 0)
            return new Dictionary<Guid, string>();

        var profiles = await uow.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(p => profileIds.Contains(p.Id))
            .Select(p => new { p.Id, p.User!.FullNameAr, p.User!.FullNameEn, p.User!.Email })
            .ToListAsync(cancellationToken);

        return profiles.ToDictionary(
            p => p.Id,
            p => GetLocalizedName(p.FullNameAr, p.FullNameEn, p.Email));
    }

    private string GetLocalizedName(string fullNameAr, string fullNameEn, string? email)
    {
        var localizedName = localizationService.GetLocalizedValue(fullNameAr, fullNameEn);
        if (!string.IsNullOrWhiteSpace(localizedName))
            return localizedName;

        var fallbackName = localizationService.GetLocalizedValue(fullNameEn, fullNameAr);
        return !string.IsNullOrWhiteSpace(fallbackName) ? fallbackName : email ?? string.Empty;
    }

    private sealed record SystemAdminLogProjection
    {
        public required Guid Id { get; init; }
        public required Guid UserProfileId { get; init; }
        public required Guid UserId { get; init; }
        public required string ActionType { get; init; }
        public string? Section { get; init; }
        public string? Notes { get; init; }
        public Guid? EntityId { get; init; }
        public Guid? AttachmentId { get; init; }
        public ReviewStatus? ReviewStatus { get; init; }
        public string? LogType { get; init; }
        public required string Source { get; init; }
        public required DateTimeOffset CreatedDate { get; init; }
    }
}

