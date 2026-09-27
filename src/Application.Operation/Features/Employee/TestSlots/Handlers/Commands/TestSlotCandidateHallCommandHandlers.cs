using Application.Operation.Features.Employee.TestSlots.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Exams;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

internal static class TestSlotCandidateHallState
{
    internal static Task LockAsync(IUnitOfWork unitOfWork, Guid slotId, CancellationToken token) =>
        unitOfWork.Context.Database.ExecuteSqlRawAsync(
            "DECLARE @result int; EXEC @result = sys.sp_getapplock @Resource = {0}, @LockMode = N'Exclusive', @LockOwner = N'Transaction', @LockTimeout = 15000; IF @result < 0 THROW 51000, 'Test slot hall operation lock unavailable', 1;",
            [$"Tawtheef.CandidateExam.TestSlot.{slotId}"], token);

    internal static async Task<(TestSessionCandidate? Candidate, IResult<Unit>? Failure)> LoadAsync(
        IUnitOfWork unitOfWork, ICurrentUserService currentUser, Guid slotId, Guid candidateId,
        CancellationToken token)
    {
        if (slotId == Guid.Empty || candidateId == Guid.Empty || !Guid.TryParse(currentUser.UserId, out var userId))
            return (null, Result.Fail<Unit>(ErrorsCodes.InvalidRequest));

        await LockAsync(unitOfWork, slotId, token);
        var slot = await unitOfWork.Context.Set<TestSlot>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == slotId, token);
        if (slot is null) return (null, Result.Fail<Unit>(ErrorsCodes.ItemNotFound));
        if (slot.StatusId != TestSlotStatusIds.Started)
            return (null, Result.Fail<Unit>(ErrorsCodes.CandidateExamSlotNotRunning));

        var isSupervisor = await unitOfWork.Context.Set<TestSlotStaff>().AnyAsync(x =>
            x.TestSlotId == slotId && x.StaffUserId == userId && x.IsActive &&
            x.RoleId == TestSlotStaffRoleIds.HallSupervisor, token);
        if (!isSupervisor)
            return (null, Result.Fail<Unit>(new Error(ErrorsCodes.UnauthorizedAction)
                .WithMetadata("StatusCode", StatusCodes.Status403Forbidden)));

        var candidate = await unitOfWork.Context.Set<TestSessionCandidate>()
            .Include(x => x.TestSession)
            .FirstOrDefaultAsync(x => x.Id == candidateId && x.TestSession!.TestSlotId == slotId, token);
        if (candidate is null) return (null, Result.Fail<Unit>(ErrorsCodes.ItemNotFound));
        if (candidate.TestSession!.StatusId != TestSessionStatusIds.InProgress)
            return (null, Result.Fail<Unit>(ErrorsCodes.CandidateExamSessionNotRunning));
        if (candidate.StatusId == TestSessionCandidateStatusIds.Cancelled ||
            candidate.StatusId == TestSessionCandidateStatusIds.Rescheduled ||
            candidate.StatusId == TestSessionCandidateStatusIds.Completed)
            return (null, Result.Fail<Unit>(ErrorsCodes.TestSlotCandidateHallStateInvalid));
        return (candidate, null);
    }
}

public sealed class MarkTestSlotCandidatePresentCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<MarkTestSlotCandidatePresentCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(MarkTestSlotCandidatePresentCommand request, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var (candidate, failure) = await TestSlotCandidateHallState.LoadAsync(unitOfWork, currentUser,
                request.TestSlotId, request.CandidateId, token);
            if (failure is not null) return failure;
            if (candidate!.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.Present)
                return Result.Ok(Unit.Value);
            var result = candidate.MarkPresent();
            if (result.IsFailed) return Result.Fail<Unit>(result.Errors);
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}

public sealed class VerifyTestSlotCandidateIdentityCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    : IRequestHandler<VerifyTestSlotCandidateIdentityCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(VerifyTestSlotCandidateIdentityCommand request, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var (candidate, failure) = await TestSlotCandidateHallState.LoadAsync(unitOfWork, currentUser,
                request.TestSlotId, request.CandidateId, token);
            if (failure is not null) return failure;
            var result = candidate!.VerifyIdentity(request.Matched, request.Notes);
            if (result.IsFailed) return Result.Fail<Unit>(result.Errors);
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}

public sealed class AuthorizeTestSlotCandidateEntryCommandHandler(
    IUnitOfWork unitOfWork, ICurrentUserService currentUser, TimeProvider timeProvider)
    : IRequestHandler<AuthorizeTestSlotCandidateEntryCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(AuthorizeTestSlotCandidateEntryCommand request, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var (candidate, failure) = await TestSlotCandidateHallState.LoadAsync(unitOfWork, currentUser,
                request.TestSlotId, request.CandidateId, token);
            if (failure is not null) return failure;
            var result = candidate!.AuthorizeEntry(timeProvider.GetUtcNow().UtcDateTime);
            if (result.IsFailed) return Result.Fail<Unit>(result.Errors);
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
