using Application.Operation.Features.Employee.Common.Access;
using Application.Operation.Features.Employee.Exceptions.DTOs;
using Application.Operation.Features.Employee.Exceptions.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services.Resources;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.Exceptions.Handlers.Queries;

public sealed class GetInvitationExceptionProofQueryHandler(
    IUnitOfWork unitOfWork,
    EmployeeJobAccessContextProvider accessContextProvider,
    IFileStorageService fileStorageService)
    : IRequestHandler<GetInvitationExceptionProofQuery, IResult<InvitationExceptionProofFileDto>>
{
    public async Task<IResult<InvitationExceptionProofFileDto>> Handle(
        GetInvitationExceptionProofQuery request,
        CancellationToken cancellationToken)
    {
        var accessibleJobs = unitOfWork.GetEntityRepository<Job>().DbSet
            .AsNoTracking()
            .ApplyJobAccessScope(accessContextProvider.GetAccess());

        var exceptionProof = await unitOfWork.GetEntityRepository<InvitationException>().DbSet
            .AsNoTracking()
            .Where(invitationException =>
                invitationException.Id == request.ExceptionId &&
                accessibleJobs.Any(job => job.Id == invitationException.JobId))
            .Select(invitationException => new
            {
                Proof = invitationException.ProofResource == null
                    ? null
                    : new
                    {
                        invitationException.ProofResource.Name,
                        invitationException.ProofResource.Type,
                        invitationException.ProofResource.Url
                    }
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (exceptionProof is null)
            return Result.Fail<InvitationExceptionProofFileDto>(NotFoundError());

        var proof = exceptionProof.Proof;
        if (proof is null)
            return Result.Fail<InvitationExceptionProofFileDto>(ProofNotFoundError());

        if (!proof.Url.StartsWith("private/", StringComparison.OrdinalIgnoreCase))
            return Result.Fail<InvitationExceptionProofFileDto>(ProofNotFoundError());

        var storedFile = await fileStorageService.OpenReadAsync(proof.Url, cancellationToken);
        if (storedFile is null)
            return Result.Fail<InvitationExceptionProofFileDto>(ProofNotFoundError());

        var fileName = Path.GetFileName(proof.Name).Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
        var contentType = string.IsNullOrWhiteSpace(proof.Type)
            ? storedFile.ContentType
            : proof.Type;

        return Result.Ok(new InvitationExceptionProofFileDto(
            storedFile.Stream,
            fileName,
            contentType));
    }

    private static Error NotFoundError() =>
        CreateError(ErrorsCodes.InvitationExceptionNotFound);

    private static Error ProofNotFoundError() =>
        CreateError(ErrorsCodes.InvitationExceptionProofNotFound);

    private static Error CreateError(string code) =>
        new Error(code)
            .WithMetadata("Code", code)
            .WithMetadata("UserMessage", code)
            .WithMetadata("StatusCode", 404);
}
