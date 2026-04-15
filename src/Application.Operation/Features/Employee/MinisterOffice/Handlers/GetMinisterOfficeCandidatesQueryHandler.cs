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
    var candidateRepo = uow.GetEntityRepository<MinisterOfficeCandidate>();
    var profileRepo   = uow.GetEntityRepository<UserProfile>();
    var search        = request.SearchTerm?.Trim();

    // ── 1. Build the base candidate query ─────────────────────────────────
    var candidates = candidateRepo.DbSet
        .AsNoTracking()
        .Where(c => !c.IsDeleted)
        .WhereIf(!request.IncludeInactive, c => c.IsFollowUpActive)
        .WhereIf(!string.IsNullOrWhiteSpace(search),
            c => EF.Functions.Like(c.Qid,        $"%{search}%") ||
                 EF.Functions.Like(c.FullNameEn,  $"%{search}%") ||
                 EF.Functions.Like(c.FullNameAr,  $"%{search}%"));
    
    // ── 3. Left-join candidates → latest profile ───────────────────────────
    var joined = candidates
        .GroupJoin(
            profileRepo.DbSet.AsNoTracking(),
            c => c.Qid,
            p => p.NationalNumber,
            (c, profiles) => new { Candidate = c, Profiles = profiles })
        .SelectMany(
            x => x.Profiles.DefaultIfEmpty(),
            (x, p) => new
            {
                x.Candidate,
                UserId          = p != null ? (Guid?)p.UserId : null,
                Status          = p != null ? (UserProfileStatus?)p.Status : null,
                GenderId        = p != null ? p.GenderId : null,
                CandidateTypeId = p != null ? p.CandidateTypeId : null,
                TargetEntityId  = p != null ? p.TargetEntityId : null,

                GenderEn        = p != null ? p.Gender!.NameEn : null,
                GenderAr        = p != null ? p.Gender!.NameAr : null,
                CandidateEn     = p != null ? p.CandidateType!.NameEn : null,
                CandidateAr     = p != null ? p.CandidateType!.NameAr : null,
                TargetEn        = p != null ? p.TargetEntity!.NameEn : null,
                TargetAr        = p != null ? p.TargetEntity!.NameAr : null,

                PhoneNumber     = p != null ? p.User!.PhoneNumber : null
            });

    // ── 4. Apply profile-column filters BEFORE pagination ─────────────────
    joined = joined
        .WhereIf(request.GenderId.HasValue,
            x => x.GenderId == request.GenderId)
        .WhereIf(request.CandidateTypeId.HasValue,
            x => x.CandidateTypeId == request.CandidateTypeId)
        .WhereIf(request.TargetEntityId.HasValue,
            x => x.TargetEntityId == request.TargetEntityId);

    joined = joined.OrderByDescending(x => x.Candidate.CreatedDate);

    var paginated = await joined.ToPaginatedListAsync(request, ct);

    // ── 5. Single pass: collect approved user-IDs + build DTOs ────────────
    var approvedUserIds = new HashSet<Guid>();
    foreach (var row in paginated.Items.Where(row => row.Status == UserProfileStatus.Approved && row.UserId.HasValue))
        approvedUserIds.Add(row.UserId!.Value);

    HashSet<Guid> profilesWithInvitations = [];
    if (approvedUserIds.Count > 0)
    {
        var ids = approvedUserIds.ToList();
        profilesWithInvitations = (await uow
            .GetEntityRepository<Invitation>().DbSet
            .AsNoTracking()
            .Where(i => ids.Contains(i.ApplicantId))
            .Select(i => i.ApplicantId)
            .Distinct()
            .ToListAsync(ct))
            .ToHashSet();
    }

    var dtoItems = paginated.Items.Select(x =>
    {
        var c      = x.Candidate;
        var status = ComputeStatus(x.Status, x.UserId, profilesWithInvitations);

        return new MinisterOfficeCandidateDto
        {
            Id              = c.Id,
            Qid             = c.Qid,
            FullNameEn      = c.FullNameEn,
            FullNameAr      = c.FullNameAr,
            NationalityEn   = c.NationalityEn,
            NationalityAr   = c.NationalityAr,
            IsFollowUpActive = c.IsFollowUpActive,
            Status          = status,
            GenderEn        = x.GenderEn,
            GenderAr        = x.GenderAr,
            CandidateTypeEn = x.CandidateEn,
            CandidateTypeAr = x.CandidateAr,
            TargetEntityEn  = x.TargetEn,
            TargetEntityAr  = x.TargetAr,
            PhoneNumber     = x.PhoneNumber ?? c.PhoneNumber,
            IsPhoneNumberFromProfile = !string.IsNullOrWhiteSpace(x.PhoneNumber),
            CreatedDate     = c.CreatedDate
        };
    }).ToList();

    return Result.Ok(new PaginatedResult<MinisterOfficeCandidateDto>(
        dtoItems,
        paginated.Metadata.TotalCount,
        paginated.Metadata.CurrentPage,
        paginated.Metadata.PageSize));
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
