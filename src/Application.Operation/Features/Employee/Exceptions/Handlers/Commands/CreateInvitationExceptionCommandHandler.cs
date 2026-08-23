using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Exceptions.Commands;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Logging;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Application.Common.Interfaces.Services.Security;
using Tawtheef.Application.Common.Services;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Application.Features.Resources.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Recruitment;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Commands;

public sealed class CreateInvitationExceptionCommandHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider jobAccessContextProvider,
    ICurrentUserService currentUserService,
    IFileStorageService fileStorageService,
    TimeProvider timeProvider,
    IMediator mediator,
    IAppLogger logger)
    : IRequestHandler<CreateInvitationExceptionCommand, IResult<CreateInvitationExceptionResultDto>>
{
    public async Task<IResult<CreateInvitationExceptionResultDto>> Handle(
        CreateInvitationExceptionCommand request,
        CancellationToken cancellationToken)
    {
        var reason = request.Reason.Trim();
        if (string.IsNullOrWhiteSpace(reason))
            return Failure(ErrorsCodes.ExceptionReasonRequired, 400);

        if (reason.Length > InvitationException.ReasonMaxLength)
            return Failure(ErrorsCodes.ExceptionReasonTooLong, 400);

        var proof = request.Proof;
        if (proof is null or { Length: 0 })
            return Failure(ErrorsCodes.EmptyFile, 400);

        if (!Guid.TryParse(currentUserService.UserId, out var currentUserId))
            return Failure(ErrorsCodes.InvalidUserIdentifier, 400);

        var candidateResult = await ResolveCandidateAsync(request.Qid, cancellationToken);
        if (candidateResult.IsFailed)
            return Result.Fail<CreateInvitationExceptionResultDto>(candidateResult.Errors);

        var applicantId = candidateResult.Value.ApplicantId;

        var jobResult = await ResolveJobAsync(request.JobId, cancellationToken);
        if (jobResult.IsFailed)
            return Result.Fail<CreateInvitationExceptionResultDto>(jobResult.Errors);

        var hasPreviousInvitation = await unitOfWork.GetEntityRepository<Invitation>().DbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(invitation =>
                    invitation.JobId == request.JobId &&
                    invitation.ApplicantId == applicantId,
                cancellationToken);

        if (hasPreviousInvitation)
            return Failure(ErrorsCodes.ExceptionInvitationAlreadyExists, 409);

        var hasPendingException = await unitOfWork.GetEntityRepository<InvitationException>().DbSet
            .AsNoTracking()
            .AnyAsync(exception =>
                    exception.JobId == request.JobId &&
                    exception.ApplicantId == applicantId &&
                    exception.Status == InvitationExceptionStatus.ReadyToSend,
                cancellationToken);

        if (hasPendingException)
            return Failure(ErrorsCodes.ExceptionRequestAlreadyExists, 409);

        var exceptionId = Guid.NewGuid();
        var uploadPath = await InvitationExceptionProofUploadPathFactory.CreateAsync(
            exceptionId,
            proof,
            cancellationToken);

        var result = await unitOfWork.ExecuteInTransactionAsync<
            Result<CreateInvitationExceptionResultDto>>(
            async ct =>
            {
                var uploadResult = await mediator.Send(
                    new UploadAttachmentCommand(
                        currentUserId,
                        uploadPath.FileId,
                        uploadPath.Path,
                        uploadPath.Hash,
                        proof),
                    ct);

                if (uploadResult.IsFailed)
                {
                    return Result.Fail<CreateInvitationExceptionResultDto>(
                        uploadResult.Errors);
                }

                var invitationException = new InvitationException
                {
                    Id = exceptionId,
                    JobId = jobResult.Value,
                    ApplicantId = applicantId,
                    InvitationId = null,
                    Reason = reason,
                    ProofResourceId = uploadResult.Value.ResourceId,
                    Status = InvitationExceptionStatus.ReadyToSend
                };

                var addResult = await unitOfWork
                    .GetEntityRepository<InvitationException>()
                    .AddAsync(invitationException, ct);

                if (addResult.IsFailed)
                {
                    logger.Error(
                        "Failed to add invitation exception {ExceptionId}: {Errors}",
                        exceptionId,
                        addResult.Errors);

                    return Failure(
                        ErrorsCodes.UnExpectedError,
                        500);
                }

                await unitOfWork.SaveChangesAsync(ct);

                return Result.Ok(
                    new CreateInvitationExceptionResultDto(
                        invitationException.Id,
                        invitationException.Status,
                        invitationException.ApplicantId,
                        invitationException.JobId));
            },
            cancellationToken);
        
        if (result.IsFailed)
        {
            await DeleteProofBlobAsync(
                uploadPath.Path,
                CancellationToken.None);
        }

        return result;
    }

    private async Task<Result<CandidateRow>> ResolveCandidateAsync(
        string qid,
        CancellationToken cancellationToken)
    {
        var normalizedQid = QidUtilities.Normalize(qid);
        if (!QidUtilities.IsValid(normalizedQid))
            return CandidateFailure(ErrorsCodes.InvalidQidFormat, 400);

        var candidate = await unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(profile =>
                profile.NationalNumber == normalizedQid &&
                profile.User != null)
            .Select(profile => new CandidateRow(profile.UserId, profile.Status))
            .SingleOrDefaultAsync(cancellationToken);

        if (candidate is null)
            return CandidateFailure(ErrorsCodes.ExceptionCandidateNotFound, 404);

        return candidate.ProfileStatus == UserProfileStatus.Approved
            ? Result.Ok(candidate)
            : CandidateFailure(ErrorsCodes.ExceptionCandidateProfileNotApproved, 409);
    }

    private async Task<Result<Guid>> ResolveJobAsync(
        Guid jobId,
        CancellationToken cancellationToken)
    {
        var job = await unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(jobAccessContextProvider.GetAccess())
            .Where(candidateJob => candidateJob.Id == jobId)
            .Select(candidateJob => new { candidateJob.Id, candidateJob.JobStatusId, candidateJob.ClosingDate })
            .SingleOrDefaultAsync(cancellationToken);

        if (job is null)
            return GuidFailure(JobMessages.JobNotFound, 404);

        var utcNow = timeProvider.GetUtcNow().UtcDateTime;
        return job.JobStatusId == JobStatusIds.Published && job.ClosingDate > utcNow
            ? Result.Ok(job.Id)
            : GuidFailure(ErrorsCodes.ExceptionJobNotOpen, 409);
    }

    private async Task DeleteProofBlobAsync(string blobPath, CancellationToken cancellationToken)
    {
        try
        {
            var deleteResult = await fileStorageService.DeleteAsync(blobPath, cancellationToken);
            if (deleteResult.IsFailed)
            {
                logger.Warning(
                    "Failed to clean up invitation exception proof blob {BlobPath}: {Errors}",
                    blobPath,
                    deleteResult.Errors);
            }
        }
        catch (Exception cleanupException)
        {
            logger.Error(
                cleanupException,
                "Unexpected error while cleaning up invitation exception proof blob {BlobPath}",
                blobPath);
        }
    }

    private static Result<CreateInvitationExceptionResultDto> Failure(string code, int statusCode) =>
        Result.Fail<CreateInvitationExceptionResultDto>(CreateError(code, statusCode));

    private static Result<CandidateRow> CandidateFailure(string code, int statusCode) =>
        Result.Fail<CandidateRow>(CreateError(code, statusCode));

    private static Result<Guid> GuidFailure(string code, int statusCode) =>
        Result.Fail<Guid>(CreateError(code, statusCode));

    private static Error CreateError(string code, int statusCode) =>
        new Error(code)
            .WithMetadata("Code", code)
            .WithMetadata("UserMessage", code)
            .WithMetadata("StatusCode", statusCode);

    private sealed record CandidateRow(Guid ApplicantId, UserProfileStatus ProfileStatus);
}
