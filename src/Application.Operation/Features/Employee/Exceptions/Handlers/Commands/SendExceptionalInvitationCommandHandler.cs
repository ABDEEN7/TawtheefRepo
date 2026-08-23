using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Exceptions.Commands;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;
using Tawtheef.Domain.Events.Operation.Employee.JobCandidates;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Commands;

public sealed class SendExceptionalInvitationCommandHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider jobAccessContextProvider,
    IInvitationExpiryConfigurationRepository invitationExpiryConfigurationRepository,
    TimeProvider timeProvider,
    IAppLogger logger)
    : IRequestHandler<SendExceptionalInvitationCommand, IResult<SendExceptionalInvitationResultDto>>
{
    private const string ActiveInvitationUniqueIndexName =
        "IX_Invitation_ApplicantId_JobId_Active";

    public async Task<IResult<SendExceptionalInvitationResultDto>> Handle(
        SendExceptionalInvitationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await unitOfWork.ExecuteInTransactionAsync<
                Result<SendExceptionalInvitationResultDto>>(
                async ct =>
                {
                    var exceptionRepository =
                        unitOfWork.GetEntityRepository<InvitationException>();

                    var claimed = await exceptionRepository.DbSet
                        .Where(exception =>
                            exception.Id == request.ExceptionId &&
                            exception.Status == InvitationExceptionStatus.ReadyToSend &&
                            exception.InvitationId == null)
                        .ExecuteUpdateAsync(
                            setters => setters.SetProperty(
                                exception => exception.Status,
                                InvitationExceptionStatus.InvitationSent),
                            ct);

                    if (claimed == 0)
                    {
                        var exists = await exceptionRepository.DbSet
                            .AsNoTracking()
                            .AnyAsync(
                                exception => exception.Id == request.ExceptionId,
                                ct);

                        return exists
                            ? Failure(
                                ErrorsCodes.InvitationExceptionNotReadyToSend,
                                409)
                            : Failure(
                                ErrorsCodes.InvitationExceptionNotFound,
                                404);
                    }

                    var invitationException = await exceptionRepository.DbSet
                        .FirstAsync(
                            exception => exception.Id == request.ExceptionId,
                            ct);

                    var proofExists = await unitOfWork
                        .GetEntityRepository<Resource>()
                        .DbSet
                        .AsNoTracking()
                        .AnyAsync(
                            resource =>
                                resource.Id == invitationException.ProofResourceId,
                            ct);

                    if (!proofExists)
                    {
                        return Failure(
                            ErrorsCodes.InvitationExceptionProofNotFound,
                            409);
                    }

                    var candidateResult = await LoadApprovedCandidateAsync(
                        invitationException.ApplicantId,
                        ct);

                    if (candidateResult.IsFailed)
                    {
                        return Result.Fail<SendExceptionalInvitationResultDto>(
                            candidateResult.Errors);
                    }

                    var jobResult = await LoadOpenJobAsync(
                        invitationException.JobId,
                        ct);

                    if (jobResult.IsFailed)
                    {
                        return Result.Fail<SendExceptionalInvitationResultDto>(
                            jobResult.Errors);
                    }

                    var hasHistoricalInvitation = await unitOfWork
                        .GetEntityRepository<Invitation>()
                        .DbSet
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .AnyAsync(
                            invitation =>
                                invitation.JobId == invitationException.JobId &&
                                invitation.ApplicantId == invitationException.ApplicantId,
                            ct);

                    if (hasHistoricalInvitation)
                    {
                        return Failure(
                            ErrorsCodes.ExceptionInvitationAlreadyExists,
                            409);
                    }

                    var expiryDays = await GetInvitationExpiryDaysAsync();
                    var currentDate =
                        DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
                    var expiresOn =
                        currentDate.AddDays(expiryDays + 1);

                    var invitation = new Invitation
                    {
                        Id = Guid.NewGuid(),
                        JobId = invitationException.JobId,
                        ApplicantId = invitationException.ApplicantId,
                        InvitationStatusId = InvitationStatusIds.NewInvitation,
                        Source = InvitationSource.Exceptional,
                        BatchNumber = Guid.NewGuid(),
                        ExpiresOn = expiresOn
                    };

                    var candidate = candidateResult.Value;
                    var job = jobResult.Value;

                    invitation.AddDomainEvent(
                        new JobCandidateInvitationSentDomainEvent(
                            invitation.Id,
                            invitation.ApplicantId,
                            candidate.Email,
                            candidate.PhoneNumber,
                            job.JobTitle,
                            expiryDays,
                            expiresOn,
                            timeProvider.GetUtcNow()));

                    var addResult = await unitOfWork
                        .GetEntityRepository<Invitation>()
                        .AddAsync(invitation, ct);

                    if (addResult.IsFailed)
                    {
                        logger.Error(
                            "Failed to add exceptional invitation for exception {ExceptionId}: {Errors}",
                            invitationException.Id,
                            addResult.Errors);

                        return Failure(
                            ErrorsCodes.UnExpectedError,
                            500);
                    }

                    invitationException.InvitationId = invitation.Id;
                    invitationException.Status =
                        InvitationExceptionStatus.InvitationSent;

                    await unitOfWork.SaveChangesAsync(ct);

                    return Result.Ok(
                        new SendExceptionalInvitationResultDto(
                            invitationException.Id,
                            invitation.Id,
                            invitationException.Status,
                            invitation.InvitationStatusId,
                            invitation.ExpiresOn));
                },
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (DbUpdateException exception)
            when (IsActiveInvitationDuplicate(exception))
        {
            return Failure(
                ErrorsCodes.ExceptionInvitationAlreadyExists,
                409);
        }
        catch (Exception exception)
        {
            logger.Error(
                exception,
                "Failed to send exceptional invitation for exception {ExceptionId}",
                request.ExceptionId);

            return Failure(
                ErrorsCodes.UnExpectedError,
                500);
        }
    }

    private async Task<Result<CandidateRow>> LoadApprovedCandidateAsync(
        Guid applicantId,
        CancellationToken cancellationToken)
    {
        var candidate = await unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(profile =>
                profile.UserId == applicantId &&
                profile.User != null)
            .Select(profile => new CandidateRow(
                profile.Status,
                profile.User!.Email,
                profile.User.PhoneNumber))
            .SingleOrDefaultAsync(cancellationToken);

        if (candidate is null)
            return CandidateFailure(ErrorsCodes.ExceptionCandidateNotFound, 404);

        return candidate.ProfileStatus == UserProfileStatus.Approved
            ? Result.Ok(candidate)
            : CandidateFailure(ErrorsCodes.ExceptionCandidateProfileNotApproved, 409);
    }

    private async Task<Result<JobRow>> LoadOpenJobAsync(
        Guid jobId,
        CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(jobAccessContextProvider.GetAccess())
            .Where(candidateJob => candidateJob.Id == jobId)
            .Select(candidateJob => new JobRow(
                candidateJob.JobStatusId,
                candidateJob.ClosingDate,
                candidateJob.JobTitle != null
                    ? candidateJob.JobTitle.JobNameEn
                    : string.Empty))
            .SingleOrDefaultAsync(cancellationToken);

        if (job is null)
            return JobFailure(JobMessages.JobNotFound, 404);

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        return job.JobStatusId == JobStatusIds.Published && job.ClosingDate > utcNow
            ? Result.Ok(job)
            : JobFailure(ErrorsCodes.ExceptionJobNotOpen, 409);
    }

    private async Task<int> GetInvitationExpiryDaysAsync()
    {
        var configuration = await invitationExpiryConfigurationRepository.GetAsync();
        return configuration.IsSuccess ? configuration.Value.ExpiryDays : 7;
    }

    private static bool IsActiveInvitationDuplicate(DbUpdateException exception)
    {
        return exception.InnerException is SqlException sqlException &&
               sqlException.Errors.Cast<SqlError>().Any(error =>
                   (error.Number is 2601 or 2627) &&
                   error.Message.Contains(ActiveInvitationUniqueIndexName, StringComparison.OrdinalIgnoreCase));
    }

    private static Result<SendExceptionalInvitationResultDto> Failure(string code, int statusCode) =>
        Result.Fail<SendExceptionalInvitationResultDto>(CreateError(code, statusCode));

    private static Result<CandidateRow> CandidateFailure(string code, int statusCode) =>
        Result.Fail<CandidateRow>(CreateError(code, statusCode));

    private static Result<JobRow> JobFailure(string code, int statusCode) =>
        Result.Fail<JobRow>(CreateError(code, statusCode));

    private static Error CreateError(string code, int statusCode) =>
        new Error(code)
            .WithMetadata("Code", code)
            .WithMetadata("UserMessage", code)
            .WithMetadata("StatusCode", statusCode);

    private sealed record CandidateRow(
        UserProfileStatus ProfileStatus,
        string? Email,
        string? PhoneNumber);

    private sealed record JobRow(
        Guid JobStatusId,
        DateTime ClosingDate,
        string JobTitle);
}
