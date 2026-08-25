using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using Application.Operation.Features.Employee.Exceptions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Queries;

public sealed class GetInvitationExceptionDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider accessContextProvider,
    ILocalizationService localizationService)
    : IRequestHandler<GetInvitationExceptionDetailsQuery, IResult<InvitationExceptionDetailsDto>>
{
    public async Task<IResult<InvitationExceptionDetailsDto>> Handle(
        GetInvitationExceptionDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var accessibleJobs = unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess());

        var details = await unitOfWork.GetEntityRepository<InvitationException>().DbSet
            .AsNoTracking()
            .Where(invitationException =>
                invitationException.Id == request.ExceptionId &&
                accessibleJobs.Any(job => job.Id == invitationException.JobId))
            .Select(invitationException => new
            {
                ExceptionId = invitationException.Id,
                invitationException.ApplicantId,
                CandidateNameAr = invitationException.Applicant != null
                    ? invitationException.Applicant.FullNameAr
                    : string.Empty,
                CandidateNameEn = invitationException.Applicant != null
                    ? invitationException.Applicant.FullNameEn
                    : string.Empty,
                Qid = invitationException.Applicant != null && invitationException.Applicant.Profile != null
                    ? invitationException.Applicant.Profile.NationalNumber ?? string.Empty
                    : string.Empty,
                invitationException.JobId,
                JobNumber = invitationException.Job != null && invitationException.Job.JobTitle != null
                    ? invitationException.Job.JobTitle.JobNumber
                    : string.Empty,
                JobTitleAr = invitationException.Job != null && invitationException.Job.JobTitle != null
                    ? invitationException.Job.JobTitle.JobNameAr
                    : string.Empty,
                JobTitleEn = invitationException.Job != null && invitationException.Job.JobTitle != null
                    ? invitationException.Job.JobTitle.JobNameEn
                    : string.Empty,
                invitationException.Reason,
                invitationException.CancellationReason,
                invitationException.Status,
                invitationException.CreatedDate,
                invitationException.UpdatedDate,
                invitationException.InvitationId,
                InvitationStatusId = invitationException.Invitation != null
                    ? (Guid?)invitationException.Invitation.InvitationStatusId
                    : null,
                InvitationStatusNameAr = invitationException.Invitation != null &&
                                         invitationException.Invitation.InvitationStatus != null
                    ? invitationException.Invitation.InvitationStatus.NameAr
                    : null,
                InvitationStatusNameEn = invitationException.Invitation != null &&
                                         invitationException.Invitation.InvitationStatus != null
                    ? invitationException.Invitation.InvitationStatus.NameEn
                    : null,
                Proof = invitationException.ProofResource == null
                    ? null
                    : new InvitationExceptionProofDto(
                        invitationException.ProofResourceId,
                        invitationException.ProofResource.Name,
                        invitationException.ProofResource.Size,
                        invitationException.ProofResource.Type)
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (details is null)
            return Result.Fail<InvitationExceptionDetailsDto>(NotFoundError());

        return Result.Ok(new InvitationExceptionDetailsDto(
            details.ExceptionId,
            new InvitationExceptionCandidateDto(
                details.ApplicantId,
                localizationService.GetLocalizedValue(details.CandidateNameAr, details.CandidateNameEn),
                details.Qid),
            new InvitationExceptionJobDto(
                details.JobId,
                details.JobNumber,
                localizationService.GetLocalizedValue(details.JobTitleAr, details.JobTitleEn)),
            details.Reason,
            details.CancellationReason,
            details.Status,
            details.CreatedDate,
            details.UpdatedDate,
            details.InvitationId,
            details.InvitationStatusId,
            details.InvitationStatusNameAr is null || details.InvitationStatusNameEn is null
                ? null
                : localizationService.GetLocalizedValue(
                    details.InvitationStatusNameAr,
                    details.InvitationStatusNameEn),
            details.Proof));
    }

    private static Error NotFoundError() =>
        new Error(ErrorsCodes.InvitationExceptionNotFound)
            .WithMetadata("Code", ErrorsCodes.InvitationExceptionNotFound)
            .WithMetadata("UserMessage", ErrorsCodes.InvitationExceptionNotFound)
            .WithMetadata("StatusCode", 404);
}
