using Application.Operation.Features.Employee.MinisterOffice.DTOs;
using Application.Operation.Features.Employee.MinisterOffice.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.MinisterOffice;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.MinisterOffice.Handlers;

public sealed class GetMinisterOfficeCandidatesQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetMinisterOfficeCandidatesQuery, IResult<PaginatedResult<MinisterOfficeCandidateDto>>>
{
    public async Task<IResult<PaginatedResult<MinisterOfficeCandidateDto>>> Handle(
        GetMinisterOfficeCandidatesQuery request,
        CancellationToken ct)
    {
        var repo = uow.GetEntityRepository<MinisterOfficeCandidate>();
        var profileDb = uow.GetEntityRepository<UserProfile>().DbSet.AsNoTracking();
        var search = request.SearchTerm?.Trim();

        var query = repo.DbSet
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .WhereIf(!request.IncludeInactive, c => c.IsFollowUpActive)
            .WhereIf(!string.IsNullOrWhiteSpace(search),
                c => EF.Functions.Like(c.Qid, $"%{search}%") ||
                     EF.Functions.Like(c.FullNameEn, $"%{search}%") ||
                     EF.Functions.Like(c.FullNameAr, $"%{search}%"))
            .OrderByDescending(c => c.CreatedDate);

        var enrichedQuery = query.Select(c => new
        {
            Candidate = c,
            Profile = profileDb
                .Where(p => p.NationalNumber == c.Qid)
                .OrderByDescending(p => p.CreatedDate)
                .Select(p => new
                {
                    p.UserId,
                    p.Status,
                    p.GenderId,
                    p.CandidateTypeId,
                    p.TargetEntityId,
                    GenderEn = p.Gender!.NameEn,
                    GenderAr = p.Gender!.NameAr,
                    CandidateEn = p.CandidateType!.NameEn,
                    CandidateAr = p.CandidateType!.NameAr,
                    TargetEn = p.TargetEntity!.NameEn,
                    TargetAr = p.TargetEntity!.NameAr,
                    PhoneNumber = p.User!.PhoneNumber
                })
                .FirstOrDefault()
        });

        // Apply filters on enriching data
        if (request.GenderId.HasValue)
            enrichedQuery = enrichedQuery.Where(x => x.Profile != null && x.Profile.GenderId == request.GenderId);

        if (request.CandidateTypeId.HasValue)
            enrichedQuery = enrichedQuery.Where(x => x.Profile != null && x.Profile.CandidateTypeId == request.CandidateTypeId);

        if (request.TargetEntityId.HasValue)
            enrichedQuery = enrichedQuery.Where(x => x.Profile != null && x.Profile.TargetEntityId == request.TargetEntityId);

        var paginated = await enrichedQuery.ToPaginatedListAsync(request, ct);

        // Check invitations for approved profiles in the page
        var approvedUserIds = paginated.Items
            .Where(x => x.Profile?.Status == UserProfileStatus.Approved)
            .Select(x => x.Profile!.UserId)
            .ToList();

        var invRepo = uow.GetEntityRepository<Invitation>();
        var profilesWithInvitations = approvedUserIds.Any()
            ? (await invRepo.DbSet
                .AsNoTracking()
                .Where(i => approvedUserIds.Contains(i.ApplicantId))
                .Select(i => i.ApplicantId)
                .Distinct()
                .ToListAsync(ct))
                .ToHashSet()
            : new HashSet<Guid>();

        var dtoItems = paginated.Items.Select(x =>
        {
            var c = x.Candidate;
            var p = x.Profile;

            var status = ComputeStatus(p?.Status, p?.UserId, profilesWithInvitations);

            return new MinisterOfficeCandidateDto
            {
                Id = c.Id,
                Qid = c.Qid,
                FullNameEn = c.FullNameEn,
                FullNameAr = c.FullNameAr,
                NationalityEn = c.NationalityEn,
                NationalityAr = c.NationalityAr,
                IsFollowUpActive = c.IsFollowUpActive,
                Status = status,
                GenderEn = p?.GenderEn,
                GenderAr = p?.GenderAr,
                CandidateTypeEn = p?.CandidateEn,
                CandidateTypeAr = p?.CandidateAr,
                TargetEntityEn = p?.TargetEn,
                TargetEntityAr = p?.TargetAr,
                PhoneNumber = p?.PhoneNumber ?? c.PhoneNumber,
                IsPhoneNumberFromProfile = !string.IsNullOrWhiteSpace(p?.PhoneNumber),
                CreatedDate = c.CreatedDate
            };
        }).ToList();

        var result = new PaginatedResult<MinisterOfficeCandidateDto>(
            dtoItems,
            paginated.Metadata.TotalCount,
            paginated.Metadata.CurrentPage,
            paginated.Metadata.PageSize);

        return Result.Ok(result);
    }

    private static MinisterOfficeCandidateStatus ComputeStatus(
        UserProfileStatus? profileStatus,
        Guid? userId,
        HashSet<Guid> profilesWithInvitations)
    {
        if (profileStatus == null)
            return MinisterOfficeCandidateStatus.NoProfile;

        if (profileStatus == UserProfileStatus.Approved)
        {
            return userId.HasValue && profilesWithInvitations.Contains(userId.Value)
                ? MinisterOfficeCandidateStatus.InvitationsReceived
                : MinisterOfficeCandidateStatus.ApprovedProfile;
        }

        return profileStatus switch
        {
            UserProfileStatus.InCreation => MinisterOfficeCandidateStatus.DraftProfile,
            UserProfileStatus.Submitted => MinisterOfficeCandidateStatus.SubmittedForApproval,
            UserProfileStatus.UnderReview => MinisterOfficeCandidateStatus.SubmittedForApproval,
            UserProfileStatus.RequiresUpdate => MinisterOfficeCandidateStatus.ReturnedForCorrection,
            _ => MinisterOfficeCandidateStatus.NoProfile
        };
    }
}
