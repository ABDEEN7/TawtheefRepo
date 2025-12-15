using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Admin.Offices.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Handlers.Commands;

public sealed class DeleteOfficeCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteOfficeCommand, IResult>
{
    public async Task<IResult> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var office = await unitOfWork.GetEntityRepository<Office>().DbSet
            .Include(o => o.SupportedCountries)
            .Include(o => o.OfficeUsers)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (office is null)
            return Result.Fail(ErrorsCodes.OfficeNotFound);

        unitOfWork.RemoveRange(office.SupportedCountries);
        unitOfWork.RemoveRange(office.OfficeUsers?.ToList());
        unitOfWork.Remove(office);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
