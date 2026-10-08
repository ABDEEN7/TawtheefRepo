using Application.Operation.Features.Employee.TestSlots.Commands;
using Application.Operation.Features.Employee.TestSlots.Services;
using FluentResults;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Lookups;

namespace Application.Operation.Features.Employee.TestSlots.Handlers.Commands;

public sealed class MarkTestSlotCandidatePresentCommandHandler(
    IUnitOfWork unitOfWork,
    TestSlotCandidateHallActionService hallActionService)
    : IRequestHandler<MarkTestSlotCandidatePresentCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(MarkTestSlotCandidatePresentCommand request, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var candidateResult =
                await hallActionService.LoadCandidateForUpdateAsync(request.TestSlotId, request.CandidateId, token);

            if (candidateResult.IsFailed)
            {
                return Result.Fail<Unit>(candidateResult.Errors);
            }

            var candidate = candidateResult.Value;
            if (candidate.AttendanceStatusId == TestSessionCandidateAttendanceStatusIds.Present)
            {
                return Result.Ok(Unit.Value);
            }

            var result = candidate.MarkPresent();
            if (result.IsFailed) return Result.Fail<Unit>(result.Errors);
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}

public sealed class VerifyTestSlotCandidateIdentityCommandHandler(
    IUnitOfWork unitOfWork,
    TestSlotCandidateHallActionService hallActionService)
    : IRequestHandler<VerifyTestSlotCandidateIdentityCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(VerifyTestSlotCandidateIdentityCommand request, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var candidateResult =
                await hallActionService.LoadCandidateForUpdateAsync(request.TestSlotId, request.CandidateId, token);

            if (candidateResult.IsFailed)
            {
                return Result.Fail<Unit>(candidateResult.Errors);
            }

            var candidate = candidateResult.Value;
            var result = candidate.VerifyIdentity(request.Matched, request.Notes);
            if (result.IsFailed) return Result.Fail<Unit>(result.Errors);
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}

public sealed class AuthorizeTestSlotCandidateEntryCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    TestSlotCandidateHallActionService hallActionService)
    : IRequestHandler<AuthorizeTestSlotCandidateEntryCommand, IResult<Unit>>
{
    public Task<IResult<Unit>> Handle(AuthorizeTestSlotCandidateEntryCommand request, CancellationToken ct) =>
        unitOfWork.ExecuteInTransactionAsync<IResult<Unit>>(async token =>
        {
            var candidateResult =
                await hallActionService.LoadCandidateForUpdateAsync(request.TestSlotId, request.CandidateId, token);

            if (candidateResult.IsFailed)
            {
                return Result.Fail<Unit>(candidateResult.Errors);
            }

            var candidate = candidateResult.Value;
            var result = candidate.AuthorizeEntry(timeProvider.GetUtcNow().UtcDateTime);
            if (result.IsFailed) return Result.Fail<Unit>(result.Errors);
            await unitOfWork.SaveChangesAsync(token);
            return Result.Ok(Unit.Value);
        }, ct);
}
