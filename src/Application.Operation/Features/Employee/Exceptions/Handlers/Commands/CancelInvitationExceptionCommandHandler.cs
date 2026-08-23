using Application.Operation.Features.Employee.Exceptions.Commands;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Commands;

public sealed class CancelInvitationExceptionCommandHandler(
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IAppLogger logger)
    : IRequestHandler<CancelInvitationExceptionCommand, IResult<CancelInvitationExceptionResultDto>>
{
    public async Task<IResult<CancelInvitationExceptionResultDto>> Handle(
        CancelInvitationExceptionCommand request,
        CancellationToken cancellationToken)
    {
        var reason = request.Reason.Trim();

        if (string.IsNullOrWhiteSpace(reason))
            return Failure(ErrorsCodes.ExceptionReasonRequired, 400);

        if (reason.Length > InvitationException.ReasonMaxLength)
            return Failure(ErrorsCodes.ExceptionReasonTooLong, 400);

        try
        {
            return await unitOfWork.ExecuteInTransactionAsync<
                Result<CancelInvitationExceptionResultDto>>(
                async ct =>
                {
                    var exceptionRepository =
                        unitOfWork.GetEntityRepository<InvitationException>();

                    var readyToSendClaimed = await exceptionRepository.DbSet
                        .Where(exception =>
                            exception.Id == request.ExceptionId &&
                            exception.Status == InvitationExceptionStatus.ReadyToSend &&
                            exception.InvitationId == null)
                        .ExecuteUpdateAsync(
                            setters => setters.SetProperty(
                                exception => exception.Status,
                                InvitationExceptionStatus.Cancelled),
                            ct);

                    if (readyToSendClaimed == 1)
                    {
                        var invitationException = await exceptionRepository.DbSet
                            .FirstAsync(
                                exception => exception.Id == request.ExceptionId,
                                ct);

                        invitationException.Cancel(reason);

                        await unitOfWork.SaveChangesAsync(ct);

                        return Result.Ok(
                            ToResult(invitationException, null));
                    }

                    var state = await exceptionRepository.DbSet
                        .AsNoTracking()
                        .Where(exception =>
                            exception.Id == request.ExceptionId)
                        .Select(exception => new
                        {
                            exception.Status, exception.InvitationId, exception.ApplicantId, exception.JobId
                        })
                        .SingleOrDefaultAsync(ct);

                    if (state is null)
                    {
                        return Failure(
                            ErrorsCodes.InvitationExceptionNotFound,
                            404);
                    }

                    if (
                        state.Status != InvitationExceptionStatus.InvitationSent ||
                        state.InvitationId is null)
                    {
                        return Failure(
                            ErrorsCodes.InvitationExceptionCannotBeCancelled,
                            409);
                    }

                    var invitationId = state.InvitationId.Value;

                    var sentClaimed = await exceptionRepository.DbSet
                        .Where(exception =>
                            exception.Id == request.ExceptionId &&
                            exception.Status == InvitationExceptionStatus.InvitationSent &&
                            exception.InvitationId == invitationId)
                        .ExecuteUpdateAsync(
                            setters => setters.SetProperty(
                                exception => exception.Status,
                                InvitationExceptionStatus.Cancelled),
                            ct);

                    if (sentClaimed == 0)
                    {
                        return Failure(
                            ErrorsCodes.InvitationExceptionCannotBeCancelled,
                            409);
                    }

                    var invitationRepository =
                        unitOfWork.GetEntityRepository<Invitation>();

                    var invitationExpired = await invitationRepository.DbSet
                        .Where(invitation =>
                            invitation.Id == invitationId &&
                            invitation.ApplicantId == state.ApplicantId &&
                            invitation.JobId == state.JobId &&
                            !invitation.IsAccepted &&
                            (
                                invitation.InvitationStatusId ==
                                InvitationStatusIds.NewInvitation ||
                                invitation.InvitationStatusId ==
                                InvitationStatusIds.Read
                            ))
                        .ExecuteUpdateAsync(
                            setters => setters.SetProperty(
                                invitation => invitation.InvitationStatusId,
                                InvitationStatusIds.Expired),
                            ct);

                    if (invitationExpired == 0)
                    {
                        return Failure(
                            ErrorsCodes.InvitationExceptionCannotBeCancelled,
                            409);
                    }

                    var trackedException = await exceptionRepository.DbSet
                        .FirstAsync(
                            exception => exception.Id == request.ExceptionId,
                            ct);

                    var trackedInvitation = await invitationRepository.DbSet
                        .FirstAsync(
                            invitation => invitation.Id == invitationId,
                            ct);

                    trackedException.Cancel(reason);
                    trackedInvitation.UpdatedDate =
                        timeProvider.GetUtcNow().UtcDateTime;

                    await unitOfWork.SaveChangesAsync(ct);

                    return Result.Ok(
                        ToResult(
                            trackedException,
                            trackedInvitation.InvitationStatusId));
                },
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.Error(
                exception,
                "Failed to cancel invitation exception {ExceptionId}",
                request.ExceptionId);

            return Failure(
                ErrorsCodes.UnExpectedError,
                500);
        }
    }

    private static CancelInvitationExceptionResultDto ToResult(
        InvitationException invitationException,
        Guid? invitationStatusId) =>
        new(
            invitationException.Id,
            invitationException.Status,
            invitationException.InvitationId,
            invitationStatusId);

    private static Result<CancelInvitationExceptionResultDto> Failure(string code, int statusCode) =>
        Result.Fail<CancelInvitationExceptionResultDto>(
            new Error(code)
                .WithMetadata("Code", code)
                .WithMetadata("UserMessage", code)
                .WithMetadata("StatusCode", statusCode));
}
