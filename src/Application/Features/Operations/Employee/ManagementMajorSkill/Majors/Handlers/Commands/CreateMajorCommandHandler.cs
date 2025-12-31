using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Commands;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.Handlers.Commands;

public class CreateMajorCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateMajorCommand, IResult<Unit>>
{
    public async Task<IResult<Unit>> Handle(CreateMajorCommand request, CancellationToken cancellationToken)
    {
        var code = "MAJOR_" + Guid.NewGuid().ToString("N")[..8].ToUpper();
        
        //check if name ar or name en already exists
        var isNameDuplicated = await uow.GetEntityRepository<Major>().DbSet.AnyAsync(x => 
            (x.NameAr == request.NameAr || x.NameEn == request.NameEn) && x.ParentId == request.ParentMajorId, cancellationToken);
        if(isNameDuplicated)
            return Result.Fail<Unit>(new Error(ErrorsCodes.MajorNameAlreadyExists));
        
        await uow.GetEntityRepository<Major>().AddAsync(new Major
        {
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            IsActive = request.IsActive,
            ParentId = request.ParentMajorId,
            BackendName = code
        });
        await uow.SaveChangesAsync(cancellationToken);
        return Result.Ok(Unit.Value);
    }
}
