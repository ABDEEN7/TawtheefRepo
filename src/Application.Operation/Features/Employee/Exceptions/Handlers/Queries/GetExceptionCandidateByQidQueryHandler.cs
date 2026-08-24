using Application.Operation.Features.Employee.Exceptions.DTOs;
using Application.Operation.Features.Employee.Exceptions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Application.Common.Utils;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Queries;

public sealed class GetExceptionCandidateByQidQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService)
    : IRequestHandler<GetExceptionCandidateByQidQuery, IResult<ExceptionCandidateLookupDto>>
{
    public async Task<IResult<ExceptionCandidateLookupDto>> Handle(
        GetExceptionCandidateByQidQuery request,
        CancellationToken cancellationToken)
    {
        var normalizedQid = QidUtilities.Normalize(request.Qid);
        if (!QidUtilities.IsValid(normalizedQid))
            return Failure(ErrorsCodes.InvalidQidFormat, 400);

        var candidate = await unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Where(profile =>
                profile.NationalNumber == normalizedQid &&
                profile.User != null)
            .Select(profile => new CandidateLookupRow(
                profile.UserId,
                profile.Id,
                profile.Gender,
                profile.User!.FullNameAr,
                profile.User.FullNameEn,
                profile.User.Email,
                profile.User.PhoneNumber,
                profile.Status))
            .SingleOrDefaultAsync(cancellationToken);

        if (candidate is null)
            return Failure(ErrorsCodes.ExceptionCandidateNotFound, 404);

        if (candidate.ProfileStatus != UserProfileStatus.Approved)
            return Failure(ErrorsCodes.ExceptionCandidateProfileNotApproved, 409);

        return Result.Ok(new ExceptionCandidateLookupDto(
            candidate.ApplicantId,
            candidate.ProfileId,
            normalizedQid,
            localizationService.GetLocalizedValue(candidate.Gender?.NameAr ?? string.Empty,
                candidate.Gender?.NameEn ?? string.Empty),
            localizationService.GetLocalizedValue(candidate.FullNameAr, candidate.FullNameEn),
            candidate.Email ?? string.Empty,
            candidate.PhoneNumber ?? string.Empty,
            candidate.ProfileStatus));
    }

    private static Result<ExceptionCandidateLookupDto> Failure(string code, int statusCode) =>
        Result.Fail<ExceptionCandidateLookupDto>(
            new Error(code)
                .WithMetadata("Code", code)
                .WithMetadata("UserMessage", code)
                .WithMetadata("StatusCode", statusCode));

    private sealed record CandidateLookupRow(
        Guid ApplicantId,
        Guid ProfileId,
        Gender? Gender,
        string FullNameAr,
        string FullNameEn,
        string? Email,
        string? PhoneNumber,
        UserProfileStatus ProfileStatus);
}
