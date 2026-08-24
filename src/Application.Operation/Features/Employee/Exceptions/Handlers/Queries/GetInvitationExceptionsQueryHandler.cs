using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using Application.Operation.Features.Employee.Exceptions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Models.Pagination;
using Tawtheef.Application.Extensions;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Queries;

public sealed class GetInvitationExceptionsQueryHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider accessContextProvider,
    ILocalizationService localizationService)
    : IRequestHandler<GetInvitationExceptionsQuery, IResult<PaginatedResult<InvitationExceptionListItemDto>>>
{
    public async Task<IResult<PaginatedResult<InvitationExceptionListItemDto>>> Handle(
        GetInvitationExceptionsQuery request,
        CancellationToken cancellationToken)
    {
        var search = request.Search?.Trim();
        var isArabic = string.Equals(
            localizationService.GetCurrentLanguage(),
            "ar",
            StringComparison.OrdinalIgnoreCase);
        var accessibleJobs = unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess());

        var exceptions = unitOfWork.GetEntityRepository<InvitationException>().DbSet
            .AsNoTracking()
            .Where(invitationException =>
                accessibleJobs.Any(job => job.Id == invitationException.JobId));

        if (request.Status.HasValue)
            exceptions = exceptions.Where(invitationException =>
                invitationException.Status == request.Status.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            exceptions = exceptions.Where(invitationException =>
                (invitationException.Applicant != null &&
                 (
                     invitationException.Applicant.FullNameAr.Contains(search) ||
                     invitationException.Applicant.FullNameEn.Contains(search) ||

                     (invitationException.Applicant.Profile != null &&
                      invitationException.Applicant.Profile.Gender != null &&
                      (
                          invitationException.Applicant.Profile.Gender.NameAr.Contains(search) ||
                          invitationException.Applicant.Profile.Gender.NameEn.Contains(search)
                      )) ||

                     (invitationException.Applicant.Profile != null &&
                      invitationException.Applicant.Profile.NationalNumber != null &&
                      invitationException.Applicant.Profile.NationalNumber.Contains(search))
                 )) ||

                (invitationException.Job != null &&
                 invitationException.Job.JobTitle != null &&
                 (
                     invitationException.Job.JobTitle.JobNameAr.Contains(search) ||
                     invitationException.Job.JobTitle.JobNameEn.Contains(search) ||
                     invitationException.Job.JobTitle.JobNumber.Contains(search)
                 )));
        }

        if (string.IsNullOrWhiteSpace(request.SortBy))
            exceptions = exceptions.OrderByDescending(x => x.CreatedDate);

        var projected = exceptions
            .Select(invitationException => new InvitationExceptionListItemDto(
                invitationException.Id,
                invitationException.ApplicantId,
                invitationException.Applicant != null
                    ? (isArabic
                        ? invitationException.Applicant.FullNameAr
                        : invitationException.Applicant.FullNameEn)
                    : string.Empty,
                invitationException.Applicant != null && invitationException.Applicant.Profile != null &&
                invitationException.Applicant.Profile.Gender != null
                    ? (isArabic
                        ? invitationException.Applicant.Profile.Gender.NameAr
                        : invitationException.Applicant.Profile.Gender.NameEn)
                    : string.Empty,
                invitationException.Applicant != null &&
                invitationException.Applicant.Profile != null
                    ? invitationException.Applicant.Profile.NationalNumber ?? string.Empty
                    : string.Empty,
                invitationException.Applicant != null
                    ? invitationException.Applicant.Email ?? string.Empty
                    : string.Empty,
                invitationException.Applicant != null
                    ? invitationException.Applicant.PhoneNumber ?? string.Empty
                    : string.Empty,
                invitationException.JobId,
                invitationException.Job != null &&
                invitationException.Job.JobTitle != null
                    ? invitationException.Job.JobTitle.JobNumber
                    : string.Empty,
                invitationException.Job != null &&
                invitationException.Job.JobTitle != null
                    ? (isArabic
                        ? invitationException.Job.JobTitle.JobNameAr
                        : invitationException.Job.JobTitle.JobNameEn)
                    : string.Empty,
                invitationException.Reason,
                invitationException.Status,
                invitationException.InvitationId,
                invitationException.Invitation != null
                    ? invitationException.Invitation.InvitationStatusId
                    : null,
                invitationException.Invitation != null &&
                invitationException.Invitation.InvitationStatus != null
                    ? (isArabic
                        ? invitationException.Invitation.InvitationStatus.NameAr
                        : invitationException.Invitation.InvitationStatus.NameEn)
                    : null,
                invitationException.CreatedDate,
                invitationException.CancellationReason,
                invitationException.ProofResource != null));

        var result = await projected.ToPaginatedListAsync(request, cancellationToken);
        return Result.Ok(result);
    }
}
