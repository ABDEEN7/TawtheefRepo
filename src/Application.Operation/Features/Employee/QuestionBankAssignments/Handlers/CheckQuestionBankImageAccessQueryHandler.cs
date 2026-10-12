using Application.Operation.Features.Employee.QuestionBankAssignments.Queries;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Security;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.QuestionsBank;

namespace Application.Operation.Features.Employee.QuestionBankAssignments.Handlers;

public sealed class CheckQuestionBankImageAccessQueryHandler(
    IUnitOfWork uow, ICurrentUserService currentUser, IHttpContextAccessor httpContext)
    : IRequestHandler<CheckQuestionBankImageAccessQuery, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(CheckQuestionBankImageAccessQuery request, CancellationToken ct)
    {
        var key = request.NormalizedBlobKey;
        if (!request.IsQuestionBankImage)
            return Result.Ok(Unit.Value);

        var user = httpContext.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return Result.Fail<Unit>(AssignmentAccess.Forbidden());

        // Catalog/list permissions do not authorize confidential question content.
        // In particular, Exams.View is also granted to ordinary employees.
        if (user is not null && new[]
            {
                PermissionKeys.QuestionBankRequests.Review,
                PermissionKeys.Exams.ViewAttemptDetails
            }.Any(permission => user.HasClaim("permission", permission)))
            return Result.Ok(Unit.Value);

        var employeeId = await AssignmentIdentity.CurrentEmployeeId(uow, currentUser, ct);
        if (employeeId is null || user?.HasClaim("permission", PermissionKeys.QuestionBankAssignments.Manage) != true)
            return Result.Fail<Unit>(AssignmentAccess.Forbidden());

        return await ValidateEmployeeAccessAsync(uow, employeeId.Value, key, ct);
    }

    // Employee commands must not reattach a retained finalized image ID to an
    // active assignment and thereby make the image readable again.
    internal static async Task<IResult<Unit>> ValidateResourceReferenceAsync(
        IUnitOfWork uow, Guid employeeId, Guid? resourceId, CancellationToken ct)
    {
        if (resourceId is null) return Result.Ok(Unit.Value);
        var url = await uow.GetEntityRepository<Resource>().DbSet.AsNoTracking()
            .Where(r => r.Id == resourceId).Select(r => r.Url).SingleOrDefaultAsync(ct);
        if (url is null) return Result.Fail<Unit>(AssignmentAccess.Forbidden());
        var image = new CheckQuestionBankImageAccessQuery(url);
        if (!image.IsQuestionBankImage)
            return Result.Ok(Unit.Value);
        return await ValidateEmployeeAccessAsync(uow, employeeId, image.NormalizedBlobKey, ct);
    }

    private static async Task<IResult<Unit>> ValidateEmployeeAccessAsync(
        IUnitOfWork uow, Guid employeeId, string key, CancellationToken ct)
    {
        var revisions = uow.GetEntityRepository<QuestionRevision>().DbSet.AsNoTracking()
            .IgnoreQueryFilters()
            .Where(r => r.Resource != null && r.Resource.Url == key);
        var revisionIds = revisions.Where(r => !r.IsDeleted && !r.Resource!.IsDeleted).Select(r => r.Id);
        var accessibleAssignments = uow.GetEntityRepository<QuestionBankAssignment>().DbSet.AsNoTracking()
            .Where(AssignmentAccess.Accessible)
            .Where(a => a.EmployeeId == employeeId && !a.IsDeleted);
        var assignmentIds = accessibleAssignments.Select(a => a.Id);
        var ownsCurrentImage = await uow.GetEntityRepository<QuestionBankRequestItem>().DbSet.AsNoTracking()
            .AnyAsync(i => !i.IsDeleted && i.StatusId != QuestionBankRequestItemStatusIds.REMOVED_FROM_REQUEST &&
                assignmentIds.Contains(i.QuestionBankAssignmentId) &&
                i.RequestId == i.QuestionBankAssignment.QuestionBankRequestId &&
                ((i.CurrentProposedRevisionId.HasValue && revisionIds.Contains(i.CurrentProposedRevisionId.Value)) ||
                 (i.OriginalRevisionId.HasValue && revisionIds.Contains(i.OriginalRevisionId.Value))), ct);
        if (ownsCurrentImage) return Result.Ok(Unit.Value);

        var ownsBaseImage = await uow.GetEntityRepository<QuestionBankVersionQuestion>().DbSet.AsNoTracking()
            .AnyAsync(v => !v.IsDeleted && revisionIds.Contains(v.QuestionRevisionId) &&
                accessibleAssignments.Any(a => a.QuestionBankRequest.RequestTypeId == QuestionBankRequestTypeIds.MAINTENANCE &&
                    a.QuestionBankRequest.BaseVersionId == v.QuestionBankVersionId), ct);
        if (ownsBaseImage) return Result.Ok(Unit.Value);

        // Allow previews only for images that have never been attached, including
        // soft-deleted revisions, so finalization cannot restore uploader access.
        var segments = key.Split('/');
        if (segments.Length == 6 && Guid.TryParse(segments[3], out var uploaderId) &&
            uploaderId == employeeId && !await revisions.AnyAsync(ct))
            return Result.Ok(Unit.Value);

        return Result.Fail<Unit>(AssignmentAccess.Forbidden());
    }

}
