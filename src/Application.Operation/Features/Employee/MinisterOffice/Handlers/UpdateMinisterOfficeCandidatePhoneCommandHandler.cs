using System.Text.RegularExpressions;
using Application.Operation.Features.Employee.MinisterOffice.Commands;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.MinisterOffice;

namespace Application.Operation.Features.Employee.MinisterOffice.Handlers;

public sealed class UpdateMinisterOfficeCandidatePhoneCommandHandler(IUnitOfWork uow)
    : IRequestHandler<UpdateMinisterOfficeCandidatePhoneCommand, IResult<Unit>>
{
    private static readonly Regex PhoneRegex = new(@"^\+974\d{8}$", RegexOptions.Compiled);

    public async Task<IResult<Unit>> Handle(
        UpdateMinisterOfficeCandidatePhoneCommand request,
        CancellationToken ct)
    {
        var phone = request.Request.PhoneNumber?.Trim() ?? string.Empty;
        if (!PhoneRegex.IsMatch(phone))
            return Result.Fail<Unit>(ErrorsCodes.InvalidPhoneFormat);

        var repo = uow.GetEntityRepository<MinisterOfficeCandidate>();
        var candidate = await repo.DbSet
            .FirstOrDefaultAsync(c => c.Id == request.CandidateId, ct);

        if (candidate is null)
            return Result.Fail<Unit>(ErrorsCodes.MinisterOfficeCandidateNotFound);

        var oldPhone = candidate.PhoneNumber;
        candidate.PhoneNumber = phone;

        // Audit log
        var auditRepo = uow.GetEntityRepository<MinisterOfficeCandidateAuditLog>();
        await auditRepo.AddAsync(new MinisterOfficeCandidateAuditLog
        {
            CandidateId = candidate.Id,
            Qid = candidate.Qid,
            Action = MinisterOfficeCandidateAuditActions.PhoneUpdated,
            Details = $"Phone changed from {oldPhone} to {phone}"
        }, ct);

        await uow.SaveChangesAsync(ct);
        return Result.Ok(Unit.Value);
    }
}
